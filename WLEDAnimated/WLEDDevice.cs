using Kevsoft.WLED;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class WLEDDevice
{
    private readonly IWLEDApiManager _apiManager;

    public int? Width
    {
        get { return _apiManager.Width; }
    }

    public int? Height
    {
        get { return _apiManager.Height; }
    }

    public bool Is2D
    {
        get { return _apiManager.Is2D; }
    }

    public WLEDDevice(IWLEDApiManager apiManager)
    {
        _apiManager = apiManager;
    }

    public string NetworkAddress { get; set; }
    public string Name { get; set; }
    private WLedRootResponse WledDevice { get; set; }

    public async Task<bool> Refresh()
    {
        try
        {
            WledDevice = await _apiManager.Connect(NetworkAddress);
            if (WledDevice != null && WledDevice.Information != null) return true;
        }
        catch
        {
        }
        return false;
    }
}