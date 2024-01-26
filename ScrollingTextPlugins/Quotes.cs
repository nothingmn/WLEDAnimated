namespace WLEDAnimated.Services;

public class Quotes
{
    public async Task<List<Quote>> Get()
    {
        var json = await (new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true
        })).GetStreamAsync("https://api.quotable.io/quotes/random");
        return await System.Text.Json.JsonSerializer.DeserializeAsync<List<Quote>> (json);
    }
}

public class Quote
{
    public string _id { get; set; }
    public string content { get; set; }
    public string author { get; set; }
    public string[] tags { get; set; }
    public string authorSlug { get; set; }
    public int length { get; set; }
    public string dateAdded { get; set; }
    public string dateModified { get; set; }
}