namespace WLEDAnimated;

public class WledDeviceDiscovery
{
    public List<WLEDDevice> Devices { get; set; } = new List<WLEDDevice>();

    public void Start(DeviceDiscovery deviceDiscovery)
    {
        Task.Factory.StartNew(async () =>
        {
            deviceDiscovery.ValidDeviceFound += (sender, e) =>
            {
                Devices.Add(e.CreatedDevice);
            };
            deviceDiscovery.StartDiscovery();
        }, TaskCreationOptions.LongRunning);
    }
}