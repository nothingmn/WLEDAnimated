using Microsoft.Extensions.Logging;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class CryptoScrollingTextPlugin : IScrollingTextPlugin
{
    private readonly ILogger<CryptoScrollingTextPlugin> _logger;

    public CryptoScrollingTextPlugin(ILogger<CryptoScrollingTextPlugin> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetTextToDisplay(string payload = null)
    {
        _logger.LogInformation("Getting crypto: {payload}...", payload);

        var bitcoinService = new ExchangeManager();
        var markets = await bitcoinService.GetStats();
        var stats = markets.data.Where(x => x.name.Equals(payload, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        if (stats != null)
        {
            var final = $"{payload}: {Math.Round(stats.PriceUsdDecimal, 2)} USD, {Math.Round(stats.ChangePercent24HrDecimal, 2)}%";
            _logger.LogInformation(final);
            return final;
        }

        return null;
    }
}