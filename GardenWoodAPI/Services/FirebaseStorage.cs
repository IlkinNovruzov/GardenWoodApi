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
                Console.Error.WriteLine($"Dosya yükleme hatası: {ex.Message}");
                throw;
            }

            var url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media&token={downloadToken}";
            return url;
        }

        //public async Task<string> UploadFileAsync(IFormFile file)
        //{
        //    if (file == null || !FileExtensions.IsImage(file))
        //    {
        //        throw new ArgumentException("File cannot be null or empty.", nameof(file));
        //    }

        //    var objectName = Path.GetRandomFileName();
        //    var contentType = file.ContentType;
        //    try
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _storageClient.UploadObjectAsync(_bucketName, objectName, contentType, stream);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Hatanın detaylarını loglama veya yazdırma
        //        Console.Error.WriteLine($"Dosya yükleme hatası: {ex.Message}");
        //        throw;
        //    }
        //    var url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{objectName}?alt=media";
        //    //  var uxrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        //    return url;
        //}
    }
}
