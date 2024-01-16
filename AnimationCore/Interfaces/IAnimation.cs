namespace AnimationCore.Interfaces;

public interface IAnimation
{
    string Name { get; set; }
    TimeSpan Duration { get; set; }

    event EventHandler Started;

    event EventHandler Stopped;

    event EventHandler Completed;

    void ScheduleStart(DateTimeOffset startTime);

    void ScheduleStop(DateTimeOffset stopTime);

    Task StartAsync(CancellationToken cancellationToken, IProgress<AnimationProgress> progress);

    void Stop();
}