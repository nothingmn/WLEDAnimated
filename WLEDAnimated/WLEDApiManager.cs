using System.Drawing;
using Kevsoft.WLED;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class WLEDApiManager : IWLEDApiManager
{
    private System.Uri _host;
    private WLedClient _client;
    private WLedRootResponse _wledDevice;

    public async Task Connect(string ipAddress)
    {
        var host = ipAddress;
        //default to http
        if (!host.ToLowerInvariant().StartsWith("http"))
        {
            host = $"http://{host}";
        }
        System.Uri.TryCreate(host, UriKind.RelativeOrAbsolute, out _host);
        // Code to establish connection to the WLED API using the _host
        _client = new WLedClient(_host.AbsoluteUri);
        _wledDevice = await _client.Get();
    }

    public void Disconnect()
    {
        // Code to disconnect from the WLED API
        _wledDevice = null;
        _client = null;
    }

    public async Task SetBrightness(int brightness)
    {
        // Code to set the brightness of the WLED device
        _wledDevice.State.Brightness = (byte)brightness;
        await _client.Post(_wledDevice.State);
    }

    public async Task ScrollingText(string text, int speed = -0)
    {
        _wledDevice.State.Segments[0].Name = text;
        var request = StateRequest.From(_wledDevice.State);

        if (speed <= 0 && request.Segments[0].EffectSpeed.HasValue) speed = request.Segments[0].EffectSpeed.Value;
        if (speed <= 0 || speed > 255) speed = 128;

        //https://github.com/Aircoookie/WLED/blob/24b60a25d5f7d944f0d41ee9feb0537fe4c6ef42/wled00/FX.h#L250
        request.Segments[0].EffectId = 122;
        request.Segments[0].EffectSpeed = speed;
        request.Segments[0].Name = text;

        await _client.Post(request);
    }

    public async Task On(int brightness = -1)
    {
        // Code to set the brightness of the WLED device
        _wledDevice.State.On = true;
        if (brightness >= 0)
        {
            _wledDevice.State.Brightness = (byte)brightness;
        }
        await _client.Post(_wledDevice.State);
    }

    public async Task Off()
    {
        // Code to set the brightness of the WLED device
        _wledDevice.State.On = false;
        await _client.Post(_wledDevice.State);
    }
}