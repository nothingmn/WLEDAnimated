namespace AnimationCore.Interfaces;

public interface ITransition
{
    IStep ToStep { get; set; }

    event EventHandler TransitionStarted;

    event EventHandler TransitionCompleted;

    TimeSpan Duration { get; set; }
    TimeSpan StartDelay { get; set; }
    TimeSpan InterTransitionDelay { get; set; }

    Task PerformTransitionAsync(CancellationToken cancellationToken, IProgress<AnimationProgress> progress, object state = null);
}