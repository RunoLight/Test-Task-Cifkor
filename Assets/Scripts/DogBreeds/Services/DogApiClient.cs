using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DogBreeds.Model;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DogBreeds.Services
{
    [UsedImplicitly]
    public sealed class DogApiClient : IDogApiClient
    {
        private readonly DogApiConfig _config;

        public DogApiClient(DogApiConfig config)
        {
            _config = config;
        }

        public async UniTask<BreedListResponse> GetBreedsAsync(CancellationToken cancellationToken)
        {
            var url = $"{_config.BaseUrl}/breeds?limit={_config.BreedLimit}";
            using var request = UnityWebRequest.Get(url);
            request.timeout = Mathf.CeilToInt(_config.RequestTimeout);

            using var registration = cancellationToken.Register(request.Abort);
            await request.SendWebRequest().WithCancellation(cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Dog API error: {request.error}");
            }

            var breeds = JsonConvert.DeserializeObject<BreedListResponse>(request.downloadHandler.text);
            if (breeds?.Data == null)
            {
                throw new Exception("Invalid breed data format");
            }

            return breeds;
        }

        public async UniTask<SingleBreedResponse> GetBreedFactsAsync(string breedId,
            CancellationToken cancellationToken)
        {
            var url = $"{_config.BaseUrl}/breeds/{breedId}/";
            using var request = UnityWebRequest.Get(url);
            request.timeout = Mathf.CeilToInt(_config.RequestTimeout);

            using var registration = cancellationToken.Register(request.Abort);
            await request.SendWebRequest().WithCancellation(cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Dog API facts error: {request.error}");
            }

            var facts = JsonConvert.DeserializeObject<SingleBreedResponse>(request.downloadHandler.text);
            if (facts?.Data == null)
            {
                throw new Exception("Invalid facts data format");
            }

            return facts;
        }
    }
}