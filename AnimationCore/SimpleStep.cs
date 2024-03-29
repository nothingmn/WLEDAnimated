﻿using AnimationCore.Interfaces;
using System.Text.Json.Serialization;

namespace AnimationCore;

public class SimpleStep : IStep
{
    public string ParentFolder { get; set; }

    public string Description { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, object, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, object, Task> Transition { get; set; }

    [JsonIgnore]
    public Func<CancellationToken, object, Task> AfterTransition { get; set; }

    //public List<Keyframe> Keyframes { get; set; } = new List<Keyframe>();
}