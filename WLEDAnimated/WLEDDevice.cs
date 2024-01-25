using Kevsoft.WLED;

namespace WLEDAnimated;

public class WLEDDevice
{
    public string NetworkAddress { get; set; }
    public string Name { get; set; }
    public WLEDApiManager ApiClient { get; set; }
    private WLedRootResponse WledDevice { get; set; }

    public async Task<bool> Refresh()
    {
        try
        {
            ApiClient = new WLEDApiManager();
            WledDevice = await ApiClient.Connect(NetworkAddress);
            if (WledDevice != null && WledDevice.Information != null) return true;
        }
        catch
        {
        }
        return false;
    }
}