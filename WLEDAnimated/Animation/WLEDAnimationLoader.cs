using AnimationCore;
using AnimationCore.Interfaces;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.Animation;

public class WLEDAnimationLoader
{
    private readonly IImageSender _sender;

    public WLEDAnimationLoader(IImageSender sender)
    {
        _sender = sender;
    }

    public Task<WLEDAnimation> LoadWLEDAnimation(System.IO.DirectoryInfo animationFolder)
    {
        var jsonPath = new FileInfo(Path.Combine(animationFolder.FullName, "Animation.json"));
        var json = File.ReadAllText(jsonPath.FullName);
        var wledAnimation = System.Text.Json.JsonSerializer.Deserialize<WLEDAnimation>(json);
        return Task.FromResult(wledAnimation);
    }

    public async Task<IAnimation> LoadAnimation(System.IO.DirectoryInfo animationFolder)
    {
        var wledAnimation = await LoadWLEDAnimation(animationFolder);
        var animation = new LinearAnimation();
        animation.Name = wledAnimation.Name;

        if (wledAnimation.Transitions != null && wledAnimation.Transitions.Any())
        {
            foreach (var transition in wledAnimation.Transitions)
            {
                var step = CreateStep(animationFolder, transition.Step);

                var t = new BasicTransition
                {
                    ToStep = step,
                    InterTransitionDelay = TimeSpan.FromMilliseconds(transition.InterTransitionDelay),
                    StartDelay = TimeSpan.FromMilliseconds(transition.StartDelay)
                };
                animation.Transitions.Add(t);
            }
        }

        return animation as IAnimation;
    }

    private IStep? CreateStep(DirectoryInfo animationFolder, Step step)
    {
        IStep createStep = null;
        var type = step.TypeName.ToLowerInvariant();
        switch (type)
        {
            case "displayimagestep":
                createStep = new DisplayImageStep(_sender)
                {
                    IPAddress = step.IPAddress,
                    Port = step.Port,
                    ImagePath = System.IO.Path.Combine(animationFolder.FullName, step.ImagePath),
                    Width = step.Width,
                    Height = step.Height,
                    Wait = step.Wait,
                    PauseBetweenFrames = step.PauseBetweenFrames,
                    Description = step.Description
                };
                break;

            case "displaytextstep":
                createStep = new DisplayTextStep()
                {
                    IPAddress = step.IPAddress,
                    Description = step.Description,
                    TextToDisplay = step.TextToDisplay,
                    Brightness = step.Brightness,
                    DurationToDisplay = TimeSpan.FromMilliseconds(step.DurationToDisplay),
                    Revert = step.Revert,
                    Speed = step.Speed,
                    YOffSet = step.YOffSet,
                    Trail = step.Trail,
                    Rotate = step.Rotate,
                    ScrollingTextType = step.ScrollingTextType,
                    Lat = step.Lat,
                    Lon = step.Lon,
                    CryptoExchange = step.CryptoExchange,
                    FontSize = step.FontSize,
                };
                break;

            case "wledstatestep":
                createStep = new WLEDStateStep()
                {
                    IPAddress = step.IPAddress,
                    Description = step.Description,
                    State = step.State,
                    DurationToDisplay = TimeSpan.FromMilliseconds(step.DurationToDisplay),
                    Revert = step.Revert
                };
                break;

            case "multistep":
                createStep = new MultiStep();
                if (step.Steps != null && step.Steps.Any())
                {
                    foreach (var s in step.Steps)
                    {
                        var subStep = CreateStep(animationFolder, s);
                        (createStep as MultiStep).Steps.Add(subStep);
                    }
                }
                break;

            default:
                throw new NotImplementedException($"The step type {type} is not implemented.");
        }

        return createStep;
    }
}