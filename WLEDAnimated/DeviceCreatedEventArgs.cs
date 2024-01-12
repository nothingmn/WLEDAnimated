namespace WLEDAnimated;

public class DeviceCreatedEventArgs
{
    public WLEDDevice CreatedDevice { get; }
    public bool RefreshRequired { get; } = true;

    public DeviceCreatedEventArgs(WLEDDevice created, bool refresh = true)
    {
        CreatedDevice = created;

        //DeviceDiscovery already made an API request to confirm that the new device is a WLED light,
        //so a refresh is only required for manually added devices
        RefreshRequired = refresh;
    }
}