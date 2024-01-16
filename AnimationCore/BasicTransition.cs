using AnimationCore.Interfaces;

namespace AnimationCore;

public class BasicTransition : ITransition
{
    public IStep ToStep { get; set; }

    public event EventHandler TransitionStarted;

    public event EventHandler TransitionCompleted;

    public TimeSpan Duration { get; set; }
    public TimeSpan StartDelay { get; set; }
    public TimeSpan InterTransitionDelay { get; set; }

    public async Task PerformTransitionAsync(CancellationToken cancellationToken, IProgress<AnimationProgress> progress)
    {
        OnTransitionStarted();

        await Task.Delay(StartDelay, cancellationToken);

        if (ToStep.BeforeTransition != null)
        {
            await ToStep.BeforeTransition(cancellationToken);
        }
        if (ToStep.Transition != null)
        {
            await ToStep.Transition(cancellationToken);
        }

        OnTransitionCompleted();

        await Task.Delay(InterTransitionDelay, cancellationToken);
    }

    protected virtual void OnTransitionStarted()
    {
        TransitionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnTransitionCompleted()
    {
        TransitionCompleted?.Invoke(this, EventArgs.Empty);
    }
}