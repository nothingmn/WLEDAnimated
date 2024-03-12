using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Coravel;
using Coravel.Invocable;
using ImageGeneration;
using Microsoft.Extensions.DependencyInjection;
using ScrollingTextPlugins;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using WLEDAnimated.Animation;
using WLEDAnimated.API.Invocables;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Interfaces.Services;
using WLEDAnimated.Printing;
using WLEDAnimated.Services;

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
        builder.Services.AddKeyedTransient<IImageConverter, ImageToDNRGBConverter>("DNRGB");
        builder.Services.AddKeyedTransient<IImageConverter, ImageToTPM2NETConverter>("TPM2NET");
        builder.Services.AddKeyedTransient<IImageConverter, ImageToDDPConverter>("DDP");

        builder.Services.AddTransient<IImageResizer, ImageSharpImageResizer>();

        builder.Services.AddTransient<IImageToConverterFactory, ImageToConverterFactory>();
        builder.Services.AddTransient<IImageSender, ImageUDPSender>();
        builder.Services.AddTransient<WLEDDevice, WLEDDevice>();

        builder.Services.AddTransient<IWLEDApiManager, WLEDApiManager>();

        builder.Services.AddTransient<MultiStep, MultiStep>();
        builder.Services.AddTransient<DisplayImageStep, DisplayImageStep>();
        builder.Services.AddTransient<WLEDStateStep, WLEDStateStep>();

        builder.Services.AddTransient<DisplayTextStep, DisplayTextStep>();
        builder.Services.AddTransient<DisplayRenderedWeatherImageStep, DisplayRenderedWeatherImageStep>();
        builder.Services.AddTransient<Weather, Weather>();
        //builder.Services.AddTransient<IBasicTemplatedImage, BasicTemplatedImage>();
        //builder.Services.AddTransient<IBasicTemplatedImage, CoreTemplatedImage>();
        builder.Services.AddTransient<IBasicTemplatedImage, HtmlTemplatedImage>();

        builder.Services.AddSingleton<Version>();
        builder.Services.AddSingleton<DeviceDiscovery>();
        builder.Services.AddSingleton<WledDeviceDiscovery>();
        builder.Services.AddTransient<EndpointConverter, EndpointConverter>();

        builder.Services.AddTransient<WLEDAnimationLoader>();
        builder.Services.AddTransient<AnimationManager>();
        builder.Services.AddTransient<AnimationInvocer>();
        builder.Services.AddTransient<PrinterAnimationInvocer>();
        builder.Services.AddTransient<AssemblyTypeProcessor>();

        builder.Services.AddTransient<IScrollingTextPluginFactory, ScrollingTextPluginFactory>();
        LoadScrollingTextPlugins(builder.Services);
        RegisterServices(builder.Services);
        RegisterPrinterServices(builder.Services);
        //throw in our basic scheduler...
        builder.Services.AddScheduler();
        builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

        //lets add some vanity config files for usability
        builder.Configuration.AddJsonFile($"appSettings-{Environment.UserName}.json", true, false);
        builder.Configuration.AddJsonFile($"appSettings-{Environment.MachineName}.json", true, false);

        //scheduler specific, in case thats your thing
        builder.Configuration.AddJsonFile("Schedule.json", true, false);

        //cant forget about a docker specific settings file
        bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        if (isRunningInDocker)
        {
            builder.Configuration.AddJsonFile($"appSettings-docker.json", true, false);
        }

        //grab the environment variables as well
        builder.Configuration.AddEnvironmentVariables();

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
        var logger = provider.GetService<ILogger<Program>>();

        Configure3DPrinters(provider);

        provider.UseScheduler(scheduler =>
    {
        var config = provider.GetService<IConfiguration>();
        var schedules = new List<SchedulerConfig>();
        config.GetSection("Scheduler").Bind(schedules);

        foreach (var schedulerConfig in schedules?.Where(schedulerConfig => schedulerConfig.Enabled))
        {
            var type = schedulerConfig.Invocable;
            var invocer = typeof(Program).Assembly.GetTypes().Where(x => x.FullName.Equals(type, StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault();
            if (invocer != null)
            {
                var instance = app.Services.GetService(invocer) as IInvocable;
                var animationInvocer = instance as AnimationInvocer;

                if (animationInvocer != null)
                {
                    animationInvocer.Animation = schedulerConfig.Animation;
                }
                var printerInvocer = instance as PrinterAnimationInvocer;

                if (printerInvocer != null)
                {
                    printerInvocer.Animation = schedulerConfig.Animation;
                    printerInvocer.PrinterId = schedulerConfig.PrinterId;
                }

                logger.LogInformation($"Adding animation {schedulerConfig.Animation} with cron {schedulerConfig.Cron}");
                scheduler.ScheduleAsync(async () => { await instance.Invoke(); }).Cron(schedulerConfig.Cron);
            }
        }
    });

        var version = app.Services.GetService<Version>();
        logger.LogInformation(version.FullVersion);

        var dd = app.Services.GetService<WledDeviceDiscovery>();
        var discover = app.Services.GetService<DeviceDiscovery>();
        dd.Start(discover);

        app.Run();
    }

    private static void RegisterPrinterServices(IServiceCollection services)
    {
        services.AddSingleton<IPrinterInstanceManager, PrinterInstanceManager>();
        services.AddSingleton<ThreeDPrinters, ThreeDPrinters>();
        services.AddTransient<ThreeDPrinterConfiguration, ThreeDPrinterConfiguration>();
        services.AddSingleton<PrinterEventAnimation, PrinterEventAnimation>();
        services.AddKeyedTransient<I3DPrinter, PrusaLinkPrinter>("PrusaLink");
        services.AddTransient<PrinterVariableReplacer, PrinterVariableReplacer>();
    }

    private static void Configure3DPrinters(IServiceProvider services)
    {
        var printerConfigs = services.GetService<ThreeDPrinters>();
        var config = services.GetService<IConfiguration>();
        var printersConfigSection = config.GetSection("3dPrinters");
        printerConfigs.Printers = printersConfigSection.Get<List<ThreeDPrinterConfiguration>>(options =>
        {
            options.ErrorOnUnknownConfiguration = true;
        });

        var mgr = services.GetService<IPrinterInstanceManager>();
        mgr.Init().Wait();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IWeather, Weather>();
        //services.AddTransient<IWeatherResponse, WeatherResponse>();
        //services.AddTransient<ISeries, Series>();
        //services.AddTransient<IWind10m, Wind10m>();
    }

    private static void LoadScrollingTextPlugins(IServiceCollection services)
    {
        var asmFile = new FileInfo(typeof(Program).Assembly.Location);
        var binFolder = new DirectoryInfo(System.IO.Path.Combine(asmFile.Directory.FullName));

        var loader = new AssemblyTypeProcessor();
        var pluginTypes = loader.ProcessTypesImplementingInterface(binFolder.FullName, typeof(IScrollingTextPlugin));

        foreach (var pluginType in pluginTypes)
        {
            services.AddKeyedTransient(typeof(IScrollingTextPlugin), pluginType.Name, pluginType);
            services.AddTransient(typeof(IScrollingTextPlugin), pluginType);
            Console.WriteLine($"Added plugin {pluginType.Name}");
        }
    }
}