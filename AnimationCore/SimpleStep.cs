using AnimationCore.Interfaces;

namespace AnimationCore;

public class SimpleStep : IStep
{
    public string Description { get; set; }

    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    public Func<CancellationToken, Task> Transition { get; set; }

    public Func<CancellationToken, Task> AfterTransition { get; set; }
    //public List<Keyframe> Keyframes { get; set; } = new List<Keyframe>();
}