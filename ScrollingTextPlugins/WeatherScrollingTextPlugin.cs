﻿using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;
using Microsoft.Extensions.Logging;

namespace ScrollingTextPlugins;

public class WeatherScrollingTextPlugin : IScrollingTextPlugin
{
    private readonly ILogger<WeatherScrollingTextPlugin> _logger;

    public WeatherScrollingTextPlugin(ILogger<WeatherScrollingTextPlugin> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetTextToDisplay(string payload = null, object state = null)
    {
        _logger.LogInformation("Getting weather: {payload}...", payload);

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
            var wr = w.DataSeries.FirstOrDefault();
            var final = $"{wr.temp2m}C, Precip:{wr.prec_type}, Clouds:{wr.CloudCover}";
            _logger.LogInformation(final);
            return final;
        }

        return null;
    }
}