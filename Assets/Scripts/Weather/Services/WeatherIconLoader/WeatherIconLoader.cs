using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Weather.Services.WeatherIconLoader
{
    public sealed class WeatherIconLoader : IWeatherIconLoader
    {
        public async UniTask<Sprite> LoadAsync(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);

            await request.SendWebRequest().WithCancellation(cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            var texture = DownloadHandlerTexture.GetContent(request);

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}