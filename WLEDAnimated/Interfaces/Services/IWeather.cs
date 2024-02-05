using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLEDAnimated.Interfaces.Services;

public interface IWeather
{
    Task<IWeatherResponse> Get(double lat, double lon);
}

public interface IWeatherResponse
{
    List<ISeries> DataSeries { get; set; }

    ISeries Current { get; }
}

public interface ISeries
{
    int timepoint { get; set; }
    int cloudcover { get; set; }
    string CloudCover { get; }
    double seeing { get; set; }
    double transparency { get; set; }
    double lifted_index { get; set; }
    double rh2m { get; set; }
    IWind10m wind10m { get; set; }
    double temp2m { get; set; }
    string prec_type { get; set; }
}

public interface IWind10m
{
    string direction { get; set; }
    int speed { get; set; }
}