using System.Text.Json.Serialization;
using AnimationCore.Interfaces;

namespace AnimationCore;

public class LinearAnimation : IAnimation
{
    public string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public List<ITransition> Transitions { get; set; } = new List<ITransition>();

    public event EventHandler Started;

    public event EventHandler Stopped;

    public event EventHandler Completed;

    private Timer _startTimer;
    private Timer _stopTimer;

    public void ScheduleStart(DateTimeOffset startTime)
    {
        var delay = startTime - DateTimeOffset.Now;
        _startTimer = new Timer(StartTimerCallback, null, delay, Timeout.InfiniteTimeSpan);
    }

    public void ScheduleStop(DateTimeOffset stopTime)
    {
        var delay = stopTime - DateTimeOffset.Now;
        _stopTimer = new Timer(StopTimerCallback, null, delay, Timeout.InfiniteTimeSpan);
    }

    private void StartTimerCallback(object state)
    {
        // Start the animation
    }

    private void StopTimerCallback(object state)
    {
        // Stop the animation
    }

    public async Task StartAsync(CancellationToken cancellationToken, IProgress<AnimationProgress> progress, object state = null)
    {
        OnStarted(state);
        foreach (var transition in Transitions)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            await transition.PerformTransitionAsync(cancellationToken, progress, state);
        }
        OnCompleted(state);
    }

    public void Stop()
    {
        OnStopped();
        // Implement stopping logic
    }

    protected virtual void OnStarted(object state = null)
    {
        Started?.Invoke(this, new AnimationEventArgs() { State = state });
    }

    protected virtual void OnStopped()
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnCompleted(object state = null)
    {
        Completed?.Invoke(this, new AnimationEventArgs() { State = state });
    }
}