using AnimationCore.Interfaces;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace WLEDAnimated.Animation;

public class MultiStep : IStep
{
    private readonly ILogger<MultiStep> _logger;

    public MultiStep(ILogger<MultiStep> logger)
    {
        _logger = logger;
        this.Transition += async (cancellationToken) =>
        {
            var tasks = new List<Task>();

            var steps = this.Steps;
            if (steps != null && steps.Any())
            {
                foreach (var step in steps)
                {
                    _logger.LogInformation("Starting Step:{stepName}, {thisId}, {stepID}", step.GetType().Name, this.GetHashCode(), step.GetHashCode());
                    tasks.Add(step.Transition(cancellationToken));
                    _logger.LogInformation("Started Step:{stepName}", step.GetType().Name);
                }
            }

            _logger.LogInformation("All steps started for animation");
            Task.WaitAll(tasks.ToArray());
            _logger.LogInformation("All steps completed for animation");
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