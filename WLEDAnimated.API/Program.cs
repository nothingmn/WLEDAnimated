using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Coravel;
using Coravel.Invocable;
using WLEDAnimated.Animation;
using WLEDAnimated.API.Invocables;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API;

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

        builder.Services.AddTransient<WLEDAnimationLoader>();
        builder.Services.AddTransient<AnimationManager>();
        builder.Services.AddTransient<AnimationInvocer>();
        builder.Services.AddScheduler();
        builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

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

        var provider = app.Services;
        provider.UseScheduler(scheduler =>
        {
            var config = provider.GetService<IConfiguration>();
            var schedules = new List<SchedulerConfig>();
            config.GetSection("Scheduler").Bind(schedules);

            foreach (var schedulerConfig in schedules)
            {
                var type = schedulerConfig.Invocable;
                var invocer = typeof(Program).Assembly.GetTypes().Where(x => x.FullName.Equals(type, StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault();
                if (invocer != null)
                {
                    var instance = app.Services.GetService(invocer) as IInvocable;
                    //var instance = ActivatorUtilities.CreateInstance(provider, invocer) as IInvocable;
                    var animationInvocer = instance as AnimationInvocer;

                    if (animationInvocer != null)
                    {
                        animationInvocer.Animation = schedulerConfig.Animation;
                        scheduler.ScheduleAsync(async () => { await instance.Invoke(); }).Cron(schedulerConfig.Cron);
                    }
                }
            }
        });

        var dd = app.Services.GetService<WledDeviceDiscovery>();
        var discover = app.Services.GetService<DeviceDiscovery>();
        dd.Start(discover);

        app.Run();
    }
}

public class SchedulerConfig
{
    public string Cron { get; set; } = "*/5 * * * *";
    public string Invocable { get; set; }
    public string Animation { get; set; }
}