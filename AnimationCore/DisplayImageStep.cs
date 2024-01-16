using AnimationCore.Interfaces;

namespace AnimationCore;

public class DisplayImageStep : IStep
{
    public string ImagePath { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; } = 21324;

    public string Description { get; set; } = "Display Image";

    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    public Func<CancellationToken, Task> Transition { get; set; }

    public Func<CancellationToken, Task> AfterTransition { get; set; }
}