using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<AssetManager>();
            builder.Services.AddTransient<WLEDApiManager>();
            builder.Services.AddTransient<ImageUDPSender>();
            builder.Services.AddTransient<IImageConverter, ImageToDNRGBConverter>();
            builder.Services.AddTransient<IImageResizer, ImageSharpImageResizer>();
            builder.Services.AddTransient<IWLEDApiManager, WLEDApiManager>();

            builder.Services.AddTransient<DeviceDiscovery>();
            builder.Services.AddSingleton<WledDeviceDiscovery>();

            var app = builder.Build();

            //            if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
            });
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseDefaultFiles();

            var dd = app.Services.GetService<WledDeviceDiscovery>();
            var discover = app.Services.GetService<DeviceDiscovery>();
            dd.Start(discover);

            app.Run();
        }
    }
}