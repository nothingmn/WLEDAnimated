using System;
using System.Threading.Tasks;
using Kevsoft.WLED;
using Tmds.MDns;

namespace WLEDAnimated;

//Discover _http._tcp services via mDNS/Zeroconf and verify they are WLED devices by sending an API call
public class DeviceDiscovery
{
    private readonly IServiceProvider _services;
    private static DeviceDiscovery Instance;
    private ServiceBrowser serviceBrowser;

    public event EventHandler<DeviceCreatedEventArgs> ValidDeviceFound;

    public DeviceDiscovery(IServiceProvider services)
    {
        _services = services;

        serviceBrowser = new ServiceBrowser();
        serviceBrowser.ServiceAdded += OnServiceAdded;
    }

    public void StartDiscovery()
    {
        serviceBrowser.StartBrowse("_http._tcp");
    }

    public void StopDiscovery()
    {
        serviceBrowser.StopBrowse();
    }

    private async void OnServiceAdded(object sender, ServiceAnnouncementEventArgs e)
    {
        var wledDevice = _services.GetService(typeof(WLEDDevice)) as WLEDDevice;
        foreach (var addr in e.Announcement.Addresses)
        {
            wledDevice.NetworkAddress = addr.ToString(); break; //only get first address
        }
        wledDevice.Name = e.Announcement.Hostname;
        if (await wledDevice.Refresh()) //check if the service is a valid WLED light
        {
            OnValidDeviceFound(new DeviceCreatedEventArgs(wledDevice, false));
        }
    }

    protected virtual void OnValidDeviceFound(DeviceCreatedEventArgs e)
    {
        ValidDeviceFound?.Invoke(this, e);
    }
}