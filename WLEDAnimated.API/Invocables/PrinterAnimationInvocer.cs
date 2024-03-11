using Coravel.Invocable;
using WLEDAnimated.Printing;

namespace WLEDAnimated.API.Invocables;

public class PrinterAnimationInvocer : IInvocable
{
    private readonly AnimationManager _animationManager;
    private readonly IPrinterInstanceManager _instanceManager;

    public PrinterAnimationInvocer(AnimationManager animationManager, IPrinterInstanceManager instanceManager)
    {
        _animationManager = animationManager;
        _instanceManager = instanceManager;
    }

    public string PrinterId { get; set; }

    public string Animation { get; set; }

    public async Task Invoke()
    {
        var printer = _instanceManager.Printers.FirstOrDefault(p => p.Instance.Id.Equals(PrinterId, StringComparison.InvariantCultureIgnoreCase));

        Console.WriteLine($"PrinterAnimationInvocer: Animation: {Animation} for Printer: {PrinterId} invoked!");
        await _animationManager.PlayAnimation(Animation, printer.Instance);
    }
}