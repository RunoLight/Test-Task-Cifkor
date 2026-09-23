using System;
using Newtonsoft.Json;

namespace DogBreeds.Model
{
    [Serializable]
    public class BreedListResponse
    {
        [JsonProperty("data")]
        public BreedData[] Data;
    }
}