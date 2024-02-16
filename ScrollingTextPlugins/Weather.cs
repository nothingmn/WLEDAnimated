using System.Text.Json.Serialization;
using WLEDAnimated.Interfaces.Services;

namespace WLEDAnimated.Services;

public class Weather : IWeather
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