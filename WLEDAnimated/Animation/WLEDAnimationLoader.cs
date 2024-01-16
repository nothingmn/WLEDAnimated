using AnimationCore;
using AnimationCore.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Intrinsics.X86;

namespace WLEDAnimated.Animation;

public class WLEDAnimationLoader
{
    public Task<IAnimation> LoadAnimation(System.IO.DirectoryInfo animationFolder)
    {
        var jsonPath = new FileInfo(Path.Combine(animationFolder.FullName, "Animation.json"));

        var json = File.ReadAllText(jsonPath.FullName);
        var wledAnimation = JsonConvert.DeserializeObject<WLEDAnimation>(json);

        var animation = new LinearAnimation();

        animation.Name = wledAnimation.Name;

        if (wledAnimation.Transitions != null && wledAnimation.Transitions.Any())
        {
            foreach (var transition in wledAnimation.Transitions)
            {
                IStep step = null;
                if (transition.Step.TypeName.Equals("DisplayImageStep", StringComparison.InvariantCultureIgnoreCase))
                {
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
                }
                if (transition.Step.TypeName.Equals("DisplayTextStep", StringComparison.InvariantCultureIgnoreCase))
                {
                    step = new DisplayTextStep()
                    {
                        IPAddress = transition.Step.IPAddress,
                        Description = transition.Step.Description,
                        TextToDisplay = transition.Step.TextToDisplay,
                        Brightness = transition.Step.Brightness,
                        DurationToDisplay = TimeSpan.FromMilliseconds(transition.Step.DurationToDisplay),
                        Revert = transition.Step.Revert
                    };
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

        return Task.FromResult(animation as IAnimation);
    }
}