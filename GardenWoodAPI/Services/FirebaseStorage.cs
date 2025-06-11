using Google.Cloud.Storage.V1;

namespace GardenWoodAPI.Services
{
    public class FirebaseStorage
    {
        private readonly IConfiguration _configuration;
        private readonly StorageClient _storageClient;
        private readonly string? _bucketName;

        public FirebaseStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            _bucketName = _configuration["Firebase:StorageBucket"];
            var credentialsPath = _configuration["Firebase:CredentialsPath"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            _storageClient = StorageClient.Create();
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || !FileExtensions.IsImage(file))
            {
                throw new ArgumentException("File cannot be null or empty.", nameof(file));
            }

            var objectName = Path.GetRandomFileName();
            var contentType = file.ContentType;
            var downloadToken = Guid.NewGuid().ToString();

            try
            {
                using var stream = file.OpenReadStream();
                var obj = await _storageClient.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
                {
                    Bucket = _bucketName,
                    Name = objectName,
                    ContentType = contentType,
                    Metadata = new Dictionary<string, string>
                {
                    { "firebaseStorageDownloadTokens", downloadToken }
                }
                }, stream);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"File upload error: {ex.Message}");
                throw;
            }

            var url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media&token={downloadToken}";
            return url;
        }
        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) throw new ArgumentException("File URL cannot be null or empty.");

            try
            {
                var uri = new Uri(fileUrl);
                var segments = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var path = uri.AbsolutePath; // /v0/b/<bucket-name>/o/<encoded-file-name>
                var objectNameEncoded = path.Split("/o/")[1]; // <encoded-file-name>
                var objectName = Uri.UnescapeDataString(objectNameEncoded);

                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"File delete error: {ex.Message}");
                throw;
            }
        }


    }
}
