using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using PrusaLink;

namespace WLEDAnimated.Printing;

public class PrusaLinkPrinter : I3DPrinter
{
    private readonly ILogger<PrusaLinkPrinter> _log;
    private readonly AnimationManager _animationManager;
    public PrusaLinkInstance Instance = null;

    public ThreeDPrinterConfiguration Configuration { get; set; }

    public PrusaLinkPrinter(ILogger<PrusaLinkPrinter> log, AnimationManager animationManager)
    {
        _log = log;
        _animationManager = animationManager;
    }

    public async Task<bool> ConnectAsync()
    {
        Instance = new PrusaLinkInstance()
        {
            Id = Configuration.ID,
            Host = Configuration.Host,
            APIKey = Configuration.ApiKey
        };

        Instance.HttpClient = new HttpClient();
        Instance.HttpClient.DefaultRequestHeaders.Add("X-Api-Key", Instance.APIKey);
        Instance.Printer = new Client(Instance.Host.ToString(), Instance.HttpClient)
        {
            Debug = true
        };
        try
        {
            Instance.Version = await Instance.Printer.VersionAsync();
            Instance.Name = Instance.Version.PrinterName;
            Instance.IsConnected = true;
            Instance.FailedToConnect = false;
            UpdateBackground();
        }
        catch (Exception e)
        {
            Instance.IsConnected = false;
            Instance.FailedToConnect = true;
            _log.LogError(e, "Could not connect to the printer:{ID}, {Host}, {ApiKey}", Configuration.ID,
                Configuration.Host, Configuration.ApiKey);
            return false;
        }

        return true;
    }

    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    private StatusPrinterState lastState = StatusPrinterState.UNKNOWN;

    private void UpdateBackground()
    {
        Task.Factory.StartNew(async (t) =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    await semaphoreSlim.WaitAsync();
                    try
                    {
                        var i = this.Instance;
                        if (i.IsConnected)
                        {
                            try
                            {
                                i.Status = await i.Printer.StatusAsync();
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }

                            try
                            {
                                i.Version = await i.Printer.VersionAsync();
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }

                            StatusJob job = null;
                            try
                            {
                                i.Job = await i.Printer.JobGETAsync();
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        await CheckAndRunAnimations(lastState, i.Status.Printer.State);
                        lastState = i.Status.Printer.State;
                    }
                    catch (System.Exception e)
                    {
                        Trace.WriteLine(e);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }
            }, TaskCreationOptions.LongRunning);
    }

    private async Task CheckAndRunAnimations(StatusPrinterState from, StatusPrinterState to)
    {
        //no transition
        if (from == to) return;
        var name = $"On{System.Enum.GetName(to)}".ToLowerInvariant();

        var transition = from c in Configuration.Animations where c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) select c;
        if (transition != null)
        {
            var animation = await _animationManager.PlayAnimation(transition.First().Animation, this);
        }
    }

    public event PrinterUpdated OnPrinterUpdated;

    private async void Update(PrusaLinkInstance instance)
    {
        if (OnPrinterUpdated != null) OnPrinterUpdated(instance);
    }

    public Task<string> GetStatus()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisconnectAsync()
    {
        throw new NotImplementedException();
    }
}

public class PrusaLinkInstance
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Host { get; set; }
    public string APIKey { get; set; }

    [JsonIgnore]
    public HttpClient HttpClient { get; set; }

    [JsonIgnore]
    public Client Printer { get; set; }

    [JsonIgnore]
    public bool IsConnected { get; set; } = false;

    [JsonIgnore]
    public bool FailedToConnect { get; set; } = false;

    [JsonIgnore]
    public PrinterStatus Status { get; set; }

    [JsonIgnore]
    public Version_ Version { get; set; }

    [JsonIgnore]
    public StatusJob Job { get; set; }

    public string Summary
    {
        get
        {
            return $"Summary";
        }
    }
}