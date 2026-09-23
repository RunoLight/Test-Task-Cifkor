using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Weather.Model;

namespace Weather.Services
{
    [UsedImplicitly]
    public sealed class WeatherApiClient : IWeatherApiClient
    {
        private readonly WeatherConfig _config;

        public WeatherApiClient(WeatherConfig config)
        {
            _config = config;
        }

        public async UniTask<WeatherForecast> GetForecastAsync(CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(_config.ApiUrl);
            using var registration = cancellationToken.Register(request.Abort);

            await request.SendWebRequest().WithCancellation(cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception($"Weather API error: {request.error}");

            var forecast = JsonConvert.DeserializeObject<WeatherForecast>(request.downloadHandler.text);
            if (forecast?.Properties?.Periods == null || forecast.Properties.Periods.Length == 0)
            {
                throw new Exception("Invalid weather data format");
            }

            return forecast;
        }
    }
}
