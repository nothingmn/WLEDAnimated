using AnimationCore.Interfaces;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Interfaces.Services;

namespace WLEDAnimated.Animation;

public class DisplayRenderedWeatherImageStep : DisplayRenderedImageStep
{
    public DisplayRenderedWeatherImageStep(ILogger<DisplayRenderedImageStep> log, IImageSender sender, IBasicTemplatedImage templatingEngine, IWeather weatherService) : base(log, sender, templatingEngine)
    {
        this.Transition += async (token) =>
        {
            this.Data = new
            {
                Lat,
                Lon
            };

            var weather = new { Lat, Lon, Weather = await weatherService.Get(Lat.Value, Lon.Value) };

            var path = Path.GetTempFileName() + ".png";
            using (var stm = await templatingEngine.GenerateImage(Template, weather, Width))
            {
                File.WriteAllBytes(path, stm.ToArray());
            }

            sender.Send(
                IPAddress,
                Port,
                path,
                new Size(Width, Height),
                0,
                (byte)Wait,
                PauseBetweenFrames,
                Iterations
            );
            await Task.CompletedTask;
        };
    }

    public double? Lat { get; set; }
    public double? Lon { get; set; }
}

public class DisplayRenderedImageStep : IStep
{
    private readonly ILogger<DisplayRenderedImageStep> _logger;

    public DisplayRenderedImageStep(ILogger<DisplayRenderedImageStep> log, IImageSender sender, IBasicTemplatedImage templatingEngine)
    {
    }

    public string Template { get; set; }
    public object Data { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; }
    public int PauseBetweenFrames { get; set; }
    public int Iterations { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Wait { get; set; }
    public string Description { get; set; } = "Display Rendered Image Step";

    [JsonIgnore]
    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> AfterTransition { get; set; }
}