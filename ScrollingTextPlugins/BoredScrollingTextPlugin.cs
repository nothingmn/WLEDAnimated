using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class BoredScrollingTextPlugin : IScrollingTextPlugin
{
    private readonly ILogger<BoredScrollingTextPlugin> _logger;

    public BoredScrollingTextPlugin(ILogger<BoredScrollingTextPlugin> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetTextToDisplay(string payload = null, object state = null)
    {
        _logger.LogInformation("Getting bored...");
        var bored = new Bored();
        var response = await bored.Get();
        var final = $"{response.type} : {response.activity}";
        _logger.LogInformation(final);
        return final;
    }
}