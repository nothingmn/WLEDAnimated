using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WLEDAnimated.Interfaces.Services;

public interface IWeather
{
    Task<WeatherResponse> Get(double lat, double lon);
}

public class WeatherResponse
{
    public string product { get; set; }
    public string init { get; set; }

    [JsonPropertyName("dataseries")]
    public List<Series> DataSeries { get; set; }

    public Series Current
    {
        get
        {
            return DataSeries.FirstOrDefault();
        }
    }
}

public class Series
{
    public int timepoint { get; set; }
    public int cloudcover { get; set; }
    public double seeing { get; set; }
    public double transparency { get; set; }
    public double lifted_index { get; set; }
    public double rh2m { get; set; }
    public Wind10m wind10m { get; set; }
    public double temp2m { get; set; }
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