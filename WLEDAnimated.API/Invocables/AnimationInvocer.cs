using Coravel.Invocable;

namespace WLEDAnimated.API.Invocables;

public class AnimationInvocer : IInvocable
{
    private readonly AnimationManager _animationManager;

    public AnimationInvocer(AnimationManager animationManager)
    {
        _animationManager = animationManager;
    }

    public string Animation { get; set; }

    public async Task Invoke()
    {
        Console.WriteLine($"AnimationInvocer '{Animation}' invoked!");
        await _animationManager.PlayAnimation(Animation);
    }
}