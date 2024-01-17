using AnimationCore.Interfaces;
using System.Net;
using Newtonsoft.Json;
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
            await apiManager.ScrollingText(TextToDisplay);
            await Task.Delay(DurationToDisplay, cancellationToken);

            if (Revert)
            {
                await apiManager.SetState(response.State);
            }

            await Task.CompletedTask;
        };
    }

    public string TextToDisplay { get; set; } = "Hello World";
    public string IPAddress { get; set; }
    public int Brightness { get; set; }
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