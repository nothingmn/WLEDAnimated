namespace WLEDAnimated.Services;

public class Weather
{
    public async Task<WeatherResponse> Get(double lat, double lon)
    {
        var json = await (new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true
        })).GetStreamAsync($"https://www.7timer.info/bin/astro.php?lon={lon}&lat={lat}&ac=0&unit=metric&output=json&tzshift=0");
        return await System.Text.Json.JsonSerializer.DeserializeAsync<WeatherResponse>(json);
    }
}

public class WeatherResponse
{
    public string product { get; set; }
    public string init { get; set; }
    public List<Datasery> dataseries { get; set; }
}

public class Datasery
{
    public int timepoint { get; set; }
    public int cloudcover { get; set; }
    public int seeing { get; set; }
    public int transparency { get; set; }
    public int lifted_index { get; set; }
    public int rh2m { get; set; }
    public Wind10m wind10m { get; set; }
    public int temp2m { get; set; }
    public string prec_type { get; set; }

    public string CloudCover
    {
        get
        {
            switch (cloudcover)
            {
                case 0:
                    return "None";

                case 1:
                    return "0%-6%";

                case 2:
                    return "6%-19%";

                case 3:
                    return "19%-31%";

                case 4:
                    return "31%-44%";

                case 5:
                    return "44%-56%";

                case 6:
                    return "56%-69%";

                case 7:
                    return "69%-81%";

                case 8:
                    return "81%-94%";

                case 9:
                    return "94%-100%";

                default:
                    return "Unknown";
            }
        }
    }
}

public class Wind10m
{
    public string direction { get; set; }
    public int speed { get; set; }
}