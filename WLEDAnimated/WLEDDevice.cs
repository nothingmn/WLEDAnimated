namespace WLEDAnimated;

public class WLEDDevice
{
    public string NetworkAddress { get; set; }
    public string Name { get; set; }
    public WLEDApiManager ApiClient { get; set; }

    public async Task<bool> Refresh()
    {
        try
        {
            ApiClient = new WLEDApiManager();
            await ApiClient.Connect(NetworkAddress);
            
            
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}