//Example from template
using System;
/*
 * Here is where we define a model for what gets sent to the front end.
 */
namespace SWR_server
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
