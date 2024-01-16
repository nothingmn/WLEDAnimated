namespace AnimationCore.Interfaces;

public interface IStep
{
    string Description { get; set; }
    Func<CancellationToken, Task> BeforeTransition { get; set; }
    Func<CancellationToken, Task> Transition { get; set; }
    Func<CancellationToken, Task> AfterTransition { get; set; }

    //List<Keyframe> Keyframes { get; set; }
}