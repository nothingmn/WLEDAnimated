using WLEDAnimated.Interfaces;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class CryptoScrollingTextPlugin : IScrollingTextPlugin
{
    public async Task<string> GetTextToDisplay(string payload = null)
    {
        var bitcoinService = new ExchangeManager();
        var markets = await bitcoinService.GetStats();
        var stats = markets.data.Where(x => x.name.Equals(payload, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        if (stats != null)
        {
            return $"{payload}: {Math.Round(stats.PriceUsdDecimal, 2)} USD, {Math.Round(stats.ChangePercent24HrDecimal, 2)}%";
        }

        return null;
    }
}