namespace WLEDAnimated.Services;

public class Bored
{
    public async Task<BoredResponse> Get()
    {
        var json = await (new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true
        })).GetStreamAsync("https://www.boredapi.com/api/activity");
        return await System.Text.Json.JsonSerializer.DeserializeAsync<BoredResponse>(json);
    }
}

public class BoredResponse
{
    public string activity { get; set; }
    public string type { get; set; }
    public int participants { get; set; }
    public double price { get; set; }
    public string link { get; set; }
    public string key { get; set; }
    public double accessibility { get; set; }
}