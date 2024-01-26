using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class WeatherScrollingTextPlugin : IScrollingTextPlugin
{
    public async Task<string> GetTextToDisplay(string payload = null)
    {
        if (string.IsNullOrWhiteSpace(payload)) return null;

        double lat;
        double lon;
        var args = payload.Split(",");

        double.TryParse(args[0], out lat);
        double.TryParse(args[1], out lon);

        var weather = new Weather();
        var w = await weather.Get(lat, lon);
        if (w != null)
        {
            var wr = w.dataseries.FirstOrDefault();
            return $"{wr.temp2m}C, Precip:{wr.prec_type}, Clouds:{wr.CloudCover}";
        }

        return null;
    }
}