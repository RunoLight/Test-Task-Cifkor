using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weather.Services.WeatherIconLoader
{
    /// <summary>
    /// Decorator with caching of result to reduce downloading and creating same Sprites <br/>
    /// No need to set TTL
    /// </summary>
    public sealed class CachingWeatherIconLoaderDecorator : IWeatherIconLoader
    {
        private readonly IWeatherIconLoader _wrappedLoader;

        private readonly Dictionary<string, UniTask<Sprite>> _cache = new();

        public CachingWeatherIconLoaderDecorator(IWeatherIconLoader wrappedLoader)
        {
            _wrappedLoader = wrappedLoader;
        }

        public UniTask<Sprite> LoadAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                return UniTask.FromException<Sprite>(new ArgumentException("URL cannot be null or empty"));
            }

            if (_cache.TryGetValue(url, out var cachedTask))
            {
                return cachedTask;
            }

            var loadTask = ExecuteAndCacheAsync(url, cancellationToken).Preserve();

            _cache.Add(url, loadTask);

            return loadTask;
        }

        private async UniTask<Sprite> ExecuteAndCacheAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                return await _wrappedLoader.LoadAsync(url, cancellationToken);
            }
            catch (Exception)
            {
                _cache.Remove(url);
                throw;
            }
        }
    }
}