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
                var exists = from d in Devices
                             where d.NetworkAddress.Equals(e.CreatedDevice.NetworkAddress,
                                 StringComparison.InvariantCultureIgnoreCase)
                             select d;
                try
                {
                    //best effort
                    //need to lock the collection first
                    if (exists == null || !exists.Any())
                    {
                        Devices.Add(e.CreatedDevice);
                    }
                }
                catch
                {
                }
            };
            deviceDiscovery.StartDiscovery();
        }, TaskCreationOptions.LongRunning);
    }
}