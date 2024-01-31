using Kevsoft.WLED;

namespace WLEDAnimated.Animation;

public class WLEDAnimation
{
    public string Name { get; set; }
    public List<Transition> Transitions { get; set; }
}

public class Transition
{
    public int StartDelay { get; set; } = 1000;
    public int InterTransitionDelay { get; set; } = 100;
    public Step Step { get; set; }
}

public class Step
{
    public string TypeName { get; set; }
    public string Description { get; set; }
    public string? Template { get; set; }
    public string ImagePath { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Wait { get; set; }
    public int PauseBetweenFrames { get; set; }
    public StateRequest State { get; set; }
    public string TextToDisplay { get; set; }
    public int Brightness { get; set; } = 128;
    public bool Revert { get; set; } = true;
    public int DurationToDisplay { get; set; } = 5000;
    public int? Speed { get; set; }
    public int? YOffSet { get; set; }
    public int? Trail { get; set; }
    public int? Rotate { get; set; }

    public int Iterations { get; set; } = 1;
    public double? Lat { get; set; }

    public double? Lon { get; set; }
    public string ScrollingTextPluginName { get; set; }
    public string ScrollingTextPluginPayload { get; set; }

    public int? FontSize { get; set; }

    public List<Step> Steps { get; set; }
}