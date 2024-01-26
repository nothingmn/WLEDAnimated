using AnimationCore;
using AnimationCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.Animation;

public class WLEDAnimationLoader
{
    private readonly IImageSender _sender;
    private readonly IWLEDApiManager _apiManager;
    private readonly IServiceProvider _services;

    public WLEDAnimationLoader(IImageSender sender, IWLEDApiManager apiManager, IServiceProvider services)
    {
        _sender = sender;
        _apiManager = apiManager;
        _services = services;
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
                var displayImageStep = _services.GetService<DisplayImageStep>();
                displayImageStep.IPAddress = step.IPAddress;
                displayImageStep.Port = step.Port;
                displayImageStep.ImagePath = System.IO.Path.Combine(animationFolder.FullName, step.ImagePath);
                displayImageStep.Width = step.Width;
                displayImageStep.Height = step.Height;
                displayImageStep.Wait = step.Wait;
                displayImageStep.PauseBetweenFrames = step.PauseBetweenFrames;
                displayImageStep.Description = step.Description;
                createStep = displayImageStep;
                break;

            case "displaytextstep":
                var displayTextStep = _services.GetService<DisplayTextStep>();
                displayTextStep.IPAddress = step.IPAddress;
                displayTextStep.Description = step.Description;
                displayTextStep.TextToDisplay = step.TextToDisplay;
                step.Brightness = step.Brightness;
                displayTextStep.DurationToDisplay = TimeSpan.FromMilliseconds(step.DurationToDisplay);
                displayTextStep.Revert = step.Revert;
                displayTextStep.Speed = step.Speed;
                displayTextStep.YOffSet = step.YOffSet;
                displayTextStep.Trail = step.Trail;
                displayTextStep.Rotate = step.Rotate;
                displayTextStep.FontSize = step.FontSize;
                displayTextStep.ScrollingTextPluginName = step.ScrollingTextPluginName;
                displayTextStep.ScrollingTextPluginPayload = step.ScrollingTextPluginPayload;
                createStep = displayTextStep;
                break;

            case "wledstatestep":
                var wledStateStep = _services.GetService<WLEDStateStep>();
                wledStateStep.IPAddress = step.IPAddress;
                wledStateStep.Description = step.Description;
                wledStateStep.State = step.State;
                wledStateStep.DurationToDisplay = TimeSpan.FromMilliseconds(step.DurationToDisplay);
                wledStateStep.Revert = step.Revert;
                createStep = wledStateStep;
                break;

            case "multistep":
                var multiStep = _services.GetService<MultiStep>();

                //createStep = new MultiStep();
                if (step.Steps != null && step.Steps.Any())
                {
                    foreach (var s in step.Steps)
                    {
                        var subStep = CreateStep(animationFolder, s);
                        multiStep.Steps.Add(subStep);
                    }
                }

                createStep = multiStep;
                break;

            default:
                throw new NotImplementedException($"The step type {type} is not implemented.");
        }

        return createStep;
    }
}