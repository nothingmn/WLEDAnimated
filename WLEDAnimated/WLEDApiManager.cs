using Kevsoft.WLED;
using Microsoft.Extensions.Logging;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class WLEDApiManager : IWLEDApiManager
{
    private readonly IScrollingTextPluginFactory _scrollingTextPluginFactory;
    private readonly ILogger<WLEDApiManager> _log;

    public WLEDApiManager(IScrollingTextPluginFactory scrollingTextPluginFactory, ILogger<WLEDApiManager> log)
    {
        _scrollingTextPluginFactory = scrollingTextPluginFactory;
        _log = log;
    }

    private System.Uri _host;
    private WLedClient _client;
    public WLedRootResponse WledDevice { get; set; }

    public int? Width
    {
        get { return WledDevice?.Information?.Leds?.Matrix?.Width; }
    }

    public int? Height
    {
        get
        {
            return WledDevice?.Information?.Leds?.Matrix?.Height;
        }
    }

    public bool Is2D
    {
        get
        {
            return WledDevice?.Information?.Leds?.Matrix?.Width > 0 && WledDevice?.Information?.Leds?.Matrix?.Height > 0;
        }
    }

    public async Task<WLedRootResponse> Connect(string ipAddress)
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
        WledDevice = await _client.Get();

        _log.LogInformation("Connected to device:{host}", _host.AbsoluteUri);

        return WledDevice;
    }

    public void Disconnect()
    {
        _log.LogInformation("Disconnected to device:{host}", _host.AbsoluteUri);
        // Code to disconnect from the WLED API
        WledDevice = null;
        _client = null;
    }

    public async Task SetBrightness(int brightness)
    {
        _log.LogInformation("Setting brightness {bri} to device:{host}", brightness, _host.AbsoluteUri);

        // Code to set the brightness of the WLED device
        WledDevice.State.Brightness = (byte)brightness;
        await _client.Post(WledDevice.State);
    }

    public async Task ScrollingText(string text, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        WledDevice.State.Segments[0].Name = text;
        var request = StateRequest.From(WledDevice.State);

        if (speed <= 0 && request.Segments[0].EffectSpeed.HasValue) speed = request.Segments[0].EffectSpeed.Value;
        if (speed <= 0 || speed > 255) speed = 128;

        //https://github.com/Aircoookie/WLED/blob/24b60a25d5f7d944f0d41ee9feb0537fe4c6ef42/wled00/FX.h#L250
        request.Segments[0].EffectId = 122;
        request.Segments[0].EffectSpeed = speed;
        request.Segments[0].Name = text;
        request.Segments[0].Offset = yOffSet;
        request.Segments[0].EffectCustomSlider1 = trail;
        request.Segments[0].EffectCustomSlider2 = fontSize;
        request.Segments[0].EffectCustomSlider3 = rotate;

        _log.LogInformation("Posting Scrolling Text '{text}' to {host}", text, _host.AbsoluteUri);

        await _client.Post(request);
    }

    public async Task ScrollingText(string scrollingTextPluginName, string scrollingTextPluginPayload, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate)
    {
        var plugin = _scrollingTextPluginFactory.LoadPluginByName(scrollingTextPluginName);
        if (plugin != null)
        {
            await ScrollingText(await plugin.GetTextToDisplay(scrollingTextPluginPayload), speed, yOffSet, trail, fontSize, rotate);
        }
    }

    public StateRequest ConvertStateResponseToRequest(StateResponse state)
    {
        return StateRequest.From(state);
    }

    public async Task SetStateFromResponse(StateResponse state)
    {
        var request = StateRequest.From(state);
        await _client.Post(request);
    }

    public async Task SetStateFromRequest(StateRequest state)
    {
        await _client.Post(state);
    }

    public async Task On(int? brightness)
    {
        // Code to set the brightness of the WLED device
        WledDevice.State.On = true;
        if (brightness.HasValue)
        {
            WledDevice.State.Brightness = (byte)brightness;
            await _client.Post(WledDevice.State);
        }
    }

    public async Task Off()
    {
        // Code to set the brightness of the WLED device
        WledDevice.State.On = false;
        await _client.Post(WledDevice.State);
    }
}