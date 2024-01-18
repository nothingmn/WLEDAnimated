using System.Drawing;
using System.Text.Json.Nodes;
using Kevsoft.WLED;
using WLEDAnimated.Animation;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Services;

namespace WLEDAnimated;

public enum ScrollingTextType
{
    Date,
    Time,
    ShortDate,
    ShortTime,
    Bored,
    Quotes,
    Weather,
    Crypto
}

public class WLEDApiManager : IWLEDApiManager
{
    private System.Uri _host;
    private WLedClient _client;
    public WLedRootResponse WledDevice { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public bool Is2D { get; set; }

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
        return WledDevice;
    }

    public void Disconnect()
    {
        // Code to disconnect from the WLED API
        WledDevice = null;
        _client = null;
    }

    public async Task SetBrightness(int brightness)
    {
        // Code to set the brightness of the WLED device
        WledDevice.State.Brightness = (byte)brightness;
        await _client.Post(WledDevice.State);
    }

    public async Task ScrollingText(string text, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate)
    {
        WledDevice.State.Segments[0].Name = text;
        var request = StateRequest.From(WledDevice.State);

        if (speed <= 0 && request.Segments[0].EffectSpeed.HasValue) speed = request.Segments[0].EffectSpeed.Value;
        if (speed <= 0 || speed > 255) speed = 128;

        if (text.StartsWith("%"))
        {
            switch (text.ToUpperInvariant())
            {
                case "%DATE%":
                    text = DateTime.Now.ToLongDateString();
                    break;

                case "%TIME%":
                    text = DateTime.Now.ToLongTimeString();
                    break;

                case "%DATE_SHORT%":
                    text = DateTime.Now.ToShortDateString();
                    break;

                case "%TIME_SHORT%":
                    text = DateTime.Now.ToShortTimeString();
                    break;

                case "%BORED%":
                    var bored = new Bored();
                    var response = await bored.Get();
                    text = $"{response.type} : {response.activity}";
                    break;

                case "%QUOTE%":
                    var quote = new Quotes();
                    var quoteResponse = (await quote.Get()).First();
                    text = $"{quoteResponse.content} - {quoteResponse.author}";
                    break;

                default:
                    break;
            }

            if (text.StartsWith("%DATE_FORMATTED|", StringComparison.InvariantCultureIgnoreCase))
            {
                var parts = text.Split("|");
                var formatString = parts[1].Replace("%", "");
                text = DateTime.Now.ToString(formatString);
            }

            if (text.StartsWith("%WEATHER|", StringComparison.InvariantCultureIgnoreCase))
            {
                var parts = text.Split("|");
                var lat = double.Parse(parts[1].Replace("%", ""));
                var lon = double.Parse(parts[2].Replace("%", ""));
                var weather = new Weather();
                var w = await weather.Get(lat, lon);
                if (w != null)
                {
                    var wr = w.dataseries.FirstOrDefault();
                    text = $"{wr.temp2m}C, Precip:{wr.prec_type}, Clouds:{wr.CloudCover}";
                }
            }

            if (text.StartsWith("%CRYPTO|", StringComparison.InvariantCultureIgnoreCase))
            {
                var name = text.Substring(8).Replace("%", "");
                var bitcoinService = new ExchangeManager();
                var markets = await bitcoinService.GetStats();
                var stats = markets.data.Where(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (stats != null)
                {
                    text = $"{name}: {Math.Round(stats.PriceUsdDecimal, 2)} USD, {Math.Round(stats.ChangePercent24HrDecimal, 2)}%";
                }
            }
        }

        //https://github.com/Aircoookie/WLED/blob/24b60a25d5f7d944f0d41ee9feb0537fe4c6ef42/wled00/FX.h#L250
        request.Segments[0].EffectId = 122;
        request.Segments[0].EffectSpeed = speed;
        request.Segments[0].Name = text;
        request.Segments[0].Offset = yOffSet;
        request.Segments[0].EffectCustomSlider1 = trail;
        request.Segments[0].EffectCustomSlider2 = fontSize;
        request.Segments[0].EffectCustomSlider3 = rotate;

        await _client.Post(request);
    }

    public async Task ScrollingText(ScrollingTextType type, double? lat, double? lon, string? cryptoexchange, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate)
    {
        switch (type)
        {
            case ScrollingTextType.Date:
                await ScrollingText("%DATE%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.Time:
                await ScrollingText("%TIME%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.ShortDate:
                await ScrollingText("%DATE_SHORT%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.ShortTime:
                await ScrollingText("%TIME_SHORT%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.Bored:
                await ScrollingText("%BORED%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.Quotes:
                await ScrollingText("%QUOTE%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.Weather:
                await ScrollingText($"%WEATHER|{lat}|{lon}%", speed, yOffSet, trail, fontSize, rotate);
                break;

            case ScrollingTextType.Crypto:
                await ScrollingText($"%CRYPTO|{cryptoexchange}%", speed, yOffSet, trail, fontSize, rotate);
                break;

            default:
                break;
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