using System;
using Newtonsoft.Json;

namespace Weather.Model
{
    /// <summary>
    /// DTO
    /// </summary>
    [Serializable]
    public class WeatherForecast
    {
        [JsonProperty("properties")]
        public WeatherProperties Properties;
    }

    [Serializable]
    public class WeatherProperties
    {
        [JsonProperty("periods")]
        public WeatherPeriod[] Periods;
    }

    [Serializable]
    public class WeatherPeriod
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("temperature")]
        public int Temperature;

        [JsonProperty("temperatureUnit")]
        public string TemperatureUnit;

        [JsonProperty("shortForecast")]
        public string ShortForecast;

        [JsonProperty("icon")]
        public string Icon;

        [JsonProperty("detailedForecast")]
        public string DetailedForecast;
    }
}