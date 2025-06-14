
using GardenWoodAPI.Model;
using GardenWoodAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace GardenWoodAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowLocalhost",
            //        policy =>
            //        {
            //            policy.WithOrigins("http://localhost:5174")  
            //                  .AllowAnyHeader()
            //                  .AllowAnyMethod();
            //        });
            //});

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<FirebaseStorage>();

            var app = builder.Build();

            //app.UseCors("AllowLocalhost");
            // Configure the HTTP request pipeline.
                app.UseSwagger();
                app.UseSwaggerUI();
                builder.Configuration.AddUserSecrets<Program>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
