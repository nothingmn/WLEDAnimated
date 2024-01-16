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

    public async Task StartAsync(CancellationToken cancellationToken, IProgress<AnimationProgress> progress)
    {
        OnStarted();
        foreach (var transition in Transitions)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            await transition.PerformTransitionAsync(cancellationToken, progress);
        }

        OnCompleted();
    }

    public void Stop()
    {
        OnStopped();
        // Implement stopping logic
    }

    protected virtual void OnStarted()
    {
        Started?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnStopped()
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnCompleted()
    {
        Completed?.Invoke(this, EventArgs.Empty);
    }
}