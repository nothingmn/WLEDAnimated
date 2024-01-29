using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;
using Microsoft.Extensions.Logging;

namespace ScrollingTextPlugins;

public class QuotesScrollingTextPlugin : IScrollingTextPlugin
{
    private readonly ILogger<QuotesScrollingTextPlugin> _logger;

    public QuotesScrollingTextPlugin(ILogger<QuotesScrollingTextPlugin> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetTextToDisplay(string payload = null)
    {
        _logger.LogInformation("Getting quote");

        var quote = new Quotes();
        var quoteResponse = (await quote.Get()).First();
        var final = $"{quoteResponse.content} - {quoteResponse.author}";
        _logger.LogInformation(final);
        return final;
    }
}