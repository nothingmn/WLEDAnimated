using System.Text.Json.Serialization;

namespace AnimationCore.Interfaces;

public interface IStep
{
    string Description { get; set; }

    [JsonIgnore]
    Func<CancellationToken, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    Func<CancellationToken, Task> Transition { get; set; }

    [JsonIgnore]
    Func<CancellationToken, Task> AfterTransition { get; set; }

    //List<Keyframe> Keyframes { get; set; }
}