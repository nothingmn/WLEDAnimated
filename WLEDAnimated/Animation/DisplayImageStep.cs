﻿using AnimationCore.Interfaces;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.Animation;

public class DisplayImageStep : IStep
{
    public DisplayImageStep(IImageSender sender)
    {
        this.Transition += async (cancellationToken, state) =>
        {
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

    public string ParentFolder { get; set; }

    public string IPAddress { get; set; }
    public int Port { get; set; } = 21324;
    public string ImagePath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Wait { get; set; } = 1;
    public int PauseBetweenFrames { get; set; } = 100;
    public int Iterations { get; set; } = 1;
    public string Description { get; set; } = "Display Image";

    [JsonIgnore]
    public Func<CancellationToken, object, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, object, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, object, Task> AfterTransition { get; set; }
}