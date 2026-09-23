using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weather.Services.WeatherIconLoader
{
    public interface IWeatherIconLoader
    {
        UniTask<Sprite> LoadAsync(string url, CancellationToken cancellationToken);
    }
}