using AnimationCore.Interfaces;
using System.Net;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;

namespace WLEDAnimated.Animation;

public class DisplayTextStep : IStep
{
    public DisplayTextStep()
    {
        this.Transition += async (cancellationToken) =>
        {
            var apiManager = new WLEDApiManager();
            var response = await apiManager.Connect(IPAddress);
            await apiManager.On(this.Brightness);
            if (string.IsNullOrWhiteSpace(TextToDisplay))
            {
                await apiManager.ScrollingText(ScrollingTextType, Lat, Lon, CryptoExchange, Speed, YOffSet, Trail, FontSize, Rotate);
            }
            else
            {
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

    public string TextToDisplay { get; set; }
    public string IPAddress { get; set; }
    public int? Brightness { get; set; }
    public int? Speed { get; set; }
    public int? YOffSet { get; set; }
    public int? Trail { get; set; }
    public int? Rotate { get; set; }

    public ScrollingTextType ScrollingTextType { get; set; } = ScrollingTextType.Text;
    public double? Lat { get; set; }
    public double? Lon { get; set; }
    public string CryptoExchange { get; set; }
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