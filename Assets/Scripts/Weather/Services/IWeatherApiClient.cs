using System.Threading;
using Cysharp.Threading.Tasks;
using Weather.Model;

namespace Weather.Services
{
    public interface IWeatherApiClient
    {
        UniTask<WeatherForecast> GetForecastAsync(CancellationToken cancellationToken);
    }
}