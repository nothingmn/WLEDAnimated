﻿using System.Text.Json.Serialization;

namespace AnimationCore.Interfaces;

public interface IStep
{
    string ParentFolder { get; set; }
    string Description { get; set; }

    [JsonIgnore]
    Func<CancellationToken, object, Task> BeforeTransition { get; set; }

    [JsonIgnore]
    Func<CancellationToken, object, Task> Transition { get; set; }

    [JsonIgnore]
    Func<CancellationToken, object, Task> AfterTransition { get; set; }

    //List<Keyframe> Keyframes { get; set; }
}