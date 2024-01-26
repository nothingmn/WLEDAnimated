using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class QuotesScrollingTextPlugin : IScrollingTextPlugin
{
    public async Task<string> GetTextToDisplay(string payload = null)
    {
        var quote = new Quotes();
        var quoteResponse = (await quote.Get()).First();
        return $"{quoteResponse.content} - {quoteResponse.author}";
    }
}