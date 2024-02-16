using AnimationCore.Interfaces;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.Animation;

public class DisplayTextStep : IStep
{
    private readonly ILogger<DisplayTextStep> _logger;

    public DisplayTextStep(IWLEDApiManager apiManager, ILogger<DisplayTextStep> logger)
    {
        _logger = logger;
        this.Transition += async (cancellationToken) =>
        {
            var response = await apiManager.Connect(IPAddress);
            await apiManager.On(this.Brightness);
            if (string.IsNullOrWhiteSpace(TextToDisplay))
            {
                _logger.LogInformation("No Text provided, lets go the plugin route...{ScrollingTextPluginName}, {ScrollingTextPluginPayload}", ScrollingTextPluginName, ScrollingTextPluginPayload);
                await apiManager.ScrollingText(ScrollingTextPluginName, ScrollingTextPluginPayload, Speed, YOffSet, Trail, FontSize, Rotate);
            }
            else
            {
                _logger.LogInformation("Static text provided '{text}'", TextToDisplay);
                await apiManager.ScrollingText(TextToDisplay, Speed, YOffSet, Trail, FontSize, Rotate);
            }
            await Task.Delay(DurationToDisplay, cancellationToken);

            if (Revert)
            {
                await apiManager.SetStateFromResponse(response.State);
            }

            await Task.CompletedTask;
        };
    }

    public string ParentFolder { get; set; }

    public string TextToDisplay { get; set; }
    public string IPAddress { get; set; }
    public int? Brightness { get; set; }
    public int? Speed { get; set; }
    public int? YOffSet { get; set; }
    public int? Trail { get; set; }
    public int? Rotate { get; set; }
    public string ScrollingTextPluginName { get; set; }

    public string ScrollingTextPluginPayload { get; set; }
    public int? FontSize { get; set; }

    public bool Revert { get; set; } = true;
    public TimeSpan DurationToDisplay { get; set; }
    public string Description { get; set; } = "Display Text";

    [JsonIgnore]
    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> AfterTransition { get; set; }
}