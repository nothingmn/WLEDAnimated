using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WLEDAnimated.Printing;

public interface IPrinterInstanceManager
{
    List<I3DPrinter> Printers { get; set; }

    Task Init();
}

public class PrinterInstanceManager : IPrinterInstanceManager
{
    public List<I3DPrinter> Printers { get; set; }
    private readonly ThreeDPrinters _printersConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PrinterInstanceManager> _log;

    public async Task Init()
    {
        if (_printersConfig.Printers != null)
        {
            Printers = new List<I3DPrinter>();
            foreach (var printer in _printersConfig.Printers)
            {
                if (printer.Enabled)
                {
                    I3DPrinter printerInstance = _serviceProvider.GetKeyedService<I3DPrinter>(printer.Type);
                    if (printerInstance != null)
                    {
                        printerInstance.Configuration = printer;
                        var connected = await printerInstance.ConnectAsync();
                        if (connected)
                        {
                            Printers.Add(printerInstance);

                            _log.LogInformation("Connected to printer {PrinterType} at {Host}", printer.Type, printer.Host);
                        }
                    }
                    else
                    {
                        _log.LogError("Could not find a printer Instance for {PrinterType}. Check to see if it is registered in our DI container.", printer.Type);
                    }
                }
            }
        }
    }

    public PrinterInstanceManager(ThreeDPrinters printers, IServiceProvider serviceProvider, ILogger<PrinterInstanceManager> log)
    {
        _printersConfig = printers;
        _serviceProvider = serviceProvider;
        _log = log;
    }
}

public delegate void PrinterUpdated(PrusaLinkInstance instance);

public interface I3DPrinter
{
    public event PrinterUpdated OnPrinterUpdated;

    public ThreeDPrinterConfiguration Configuration { get; set; }

    public Task<bool> ConnectAsync();

    PrusaLinkInstance Instance { get; set; }
}