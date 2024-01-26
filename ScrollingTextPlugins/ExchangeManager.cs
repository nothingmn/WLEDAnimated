namespace WLEDAnimated.Services;

public class ExchangeManager
{
    public async Task<CoinStats> GetStats()
    {
        var json = await (new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true
        })).GetStreamAsync("https://api.coincap.io/v2/assets");
        return await System.Text.Json.JsonSerializer.DeserializeAsync<CoinStats>(json);
    }
}

public class CoinStats
{
    public List<Datum> data { get; set; }
    public long timestamp { get; set; }
}

public class Datum
{
    public string id { get; set; }
    public string rank { get; set; }
    public string symbol { get; set; }
    public string name { get; set; }
    public string supply { get; set; }
    public string maxSupply { get; set; }
    public string marketCapUsd { get; set; }
    public string volumeUsd24Hr { get; set; }
    public string priceUsd { get; set; }
    public string changePercent24Hr { get; set; }
    public string vwap24Hr { get; set; }
    public string explorer { get; set; }



    public decimal SupplyDecimal
    {
        get
        {
            return decimal.Parse(supply);
        }
    }

    public decimal MaxSupplyDecimal
    {
        get
        {
            return decimal.Parse(maxSupply);
        }
    }

    public decimal MarketCapUsdDecimal
    {
        get
        {
            return decimal.Parse(marketCapUsd);
        }
    }

    public decimal VolumeUsd24HrDecimal
    {
        get
        {
            return decimal.Parse(volumeUsd24Hr);
        }
    }

    public decimal PriceUsdDecimal
    {
        get
        {
            return decimal.Parse(priceUsd);
        }
    }

    public decimal ChangePercent24HrDecimal
    {
        get
        {
            return decimal.Parse(changePercent24Hr);
        }
    }

    public decimal Vwap24HrDecimal
    {
        get
        {
            return decimal.Parse(vwap24Hr);
        }
    }


}