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

    public async Task PerformTransitionAsync(CancellationToken cancellationToken, IProgress<AnimationProgress> progress, object state = null)
    {
        OnTransitionStarted(state);

        await Task.Delay(StartDelay, cancellationToken);

        if (ToStep.BeforeTransition != null)
        {
            await ToStep.BeforeTransition(cancellationToken, state);
        }
        if (ToStep.Transition != null)
        {
            await ToStep.Transition(cancellationToken, state);
        }

        OnTransitionCompleted(state);

        await Task.Delay(InterTransitionDelay, cancellationToken);
    }

    protected virtual void OnTransitionStarted(object state = null)
    {
        TransitionStarted?.Invoke(this, new AnimationEventArgs() { State = state });
    }

    protected virtual void OnTransitionCompleted(object state = null)
    {
        TransitionCompleted?.Invoke(this, new AnimationEventArgs() { State = state });
    }
}