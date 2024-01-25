using AnimationCore.Interfaces;
using System.Net;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;

namespace WLEDAnimated.Animation;

public class MultiStep : IStep
{
    public MultiStep()
    {
        this.Transition += async (cancellationToken) =>
        {
            var tasks = new List<Task>();

            var steps = this.Steps;
            if (steps != null && steps.Any())
            {
                foreach (var step in steps)
                {
                    tasks.Add(step.Transition(cancellationToken));
                }
            }

            Task.WaitAll(tasks.ToArray());
        };
    }

    public string Description { get; set; } = "Display Text";

    public List<IStep> Steps { get; set; } = new List<IStep>();

    [JsonIgnore]
    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> AfterTransition { get; set; }
}