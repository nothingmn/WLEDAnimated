using AnimationCore;
using AnimationCore.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Intrinsics.X86;

namespace WLEDAnimated.Animation;

public class WLEDAnimationLoader
{
    public Task<WLEDAnimation> LoadWLEDAnimation(System.IO.DirectoryInfo animationFolder)
    {
        var jsonPath = new FileInfo(Path.Combine(animationFolder.FullName, "Animation.json"));
        var json = File.ReadAllText(jsonPath.FullName);
        var wledAnimation = JsonConvert.DeserializeObject<WLEDAnimation>(json);
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
                IStep step = null;
                var type = transition.Step.TypeName.ToLowerInvariant();
                switch (type)
                {
                    case "displayimagestep":
                        step = new DisplayImageStep
                        {
                            IPAddress = transition.Step.IPAddress,
                            Port = transition.Step.Port,
                            ImagePath = System.IO.Path.Combine(animationFolder.FullName, transition.Step.ImagePath),
                            Width = transition.Step.Width,
                            Height = transition.Step.Height,
                            Wait = transition.Step.Wait,
                            PauseBetweenFrames = transition.Step.PauseBetweenFrames,
                            Description = transition.Step.Description
                        };
                        break;

                    case "displaytextstep":
                        step = new DisplayTextStep()
                        {
                            IPAddress = transition.Step.IPAddress,
                            Description = transition.Step.Description,
                            TextToDisplay = transition.Step.TextToDisplay,
                            Brightness = transition.Step.Brightness,
                            DurationToDisplay = TimeSpan.FromMilliseconds(transition.Step.DurationToDisplay),
                            Revert = transition.Step.Revert
                        };
                        break;

                    case "wledstatestep":
                        step = new WLEDStateStep()
                        {
                            IPAddress = transition.Step.IPAddress,
                            Description = transition.Step.Description,
                            State = transition.Step.State,
                            DurationToDisplay = TimeSpan.FromMilliseconds(transition.Step.DurationToDisplay),
                            Revert = transition.Step.Revert
                        };
                        break;

                    default:
                        throw new NotImplementedException($"The step type {type} is not implemented.");
                }

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
}