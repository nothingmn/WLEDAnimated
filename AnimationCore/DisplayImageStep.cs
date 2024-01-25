using AnimationCore.Interfaces;
using System.Text.Json.Serialization;

namespace AnimationCore;

public class DisplayImageStep : IStep
{
    public string ImagePath { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; } = 21324;
    public string Description { get; set; } = "Display Image";

    [JsonIgnore]
    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, Task> AfterTransition { get; set; }
}