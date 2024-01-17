using AnimationCore.Interfaces;
using System.Net;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using Kevsoft.WLED;

namespace WLEDAnimated.Animation;

public class WLEDStateStep : IStep
{
    public WLEDStateStep()
    {
        this.Transition += async (cancellationToken) =>
        {
            var apiManager = new WLEDApiManager();
            var response = await apiManager.Connect(IPAddress);

            await apiManager.SetStateFromRequest(State);
            await Task.Delay(DurationToDisplay, cancellationToken);

            if (Revert)
            {
                await apiManager.SetStateFromResponse(response.State);
            }

            await Task.CompletedTask;
        };
    }

    public StateRequest State { get; set; }
    public string IPAddress { get; set; }
    public bool Revert { get; set; }
    public TimeSpan DurationToDisplay { get; set; }
    public string Description { get; set; } = "Send a WLED State change";

    [JsonIgnore]
    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> AfterTransition { get; set; }
}