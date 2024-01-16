using AnimationCore.Interfaces;
using System.Net;
using SixLabors.ImageSharp;

namespace WLEDAnimated.Animation;

public class DisplayImageStep : IStep
{
    public DisplayImageStep()
    {
        this.Transition += async (cancellationToken) =>
        {
            var sender = new ImageUDPSender();
            sender.Send(
                IPAddress,
                Port,
                ImagePath,
                new Size(Width, Height),
                0,
                (byte)Wait,
                PauseBetweenFrames,
                Iterations
            );
            await Task.CompletedTask;
        };
    }

    public string IPAddress { get; set; }
    public int Port { get; set; } = 21324;
    public string ImagePath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Wait { get; set; } = 1;
    public int PauseBetweenFrames { get; set; } = 100;
    public int Iterations { get; set; } = 1;
    public string Description { get; set; } = "Display Image";

    public Func<CancellationToken, Task> BeforeTransition { get; set; }

    public Func<CancellationToken, Task> Transition { get; set; }

    public Func<CancellationToken, Task> AfterTransition { get; set; }
}