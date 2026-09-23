using System;
using Newtonsoft.Json;

namespace DogBreeds.Model
{
    [Serializable]
    public class BreedData
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("attributes")]
        public BreedAttributes Attributes;
    }

    [Serializable]
    public class BreedAttributes
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("origin")]
        public BreedOrigin Origin;

        [JsonProperty("life")]
        public IntRange Life;

        [JsonProperty("male_weight")]
        public IntRange MaleWeight;

        [JsonProperty("female_weight")]
        public IntRange FemaleWeight;

        [JsonProperty("male_height")]
        public IntRange MaleHeight;

        [JsonProperty("female_height")]
        public IntRange FemaleHeight;

        [JsonProperty("traits")]
        public BreedTraits Traits;
    }

    [Serializable]
    public class BreedOrigin
    {
        [JsonProperty("country")]
        public string Country;

        [JsonProperty("region")]
        public string Region;

        [JsonProperty("era")]
        public string Era;
    }

    [Serializable]
    public class IntRange
    {
        [JsonProperty("min")]
        public int Min;

        [JsonProperty("max")]
        public int Max;
    }
    
    [Serializable]
    public class BreedTraits
    {
        [JsonProperty("energy")]
        public int Energy;

        [JsonProperty("trainability")]
        public int Trainability;

        [JsonProperty("barking")]
        public int Barking;

        [JsonProperty("grooming")]
        public int Grooming;

        [JsonProperty("shedding")]
        public int Shedding;

        [JsonProperty("drooling")]
        public int Drooling;

        [JsonProperty("good_with_children")]
        public int GoodWithChildren;

        [JsonProperty("good_with_dogs")]
        public int GoodWithDogs;

        [JsonProperty("good_with_strangers")]
        public int GoodWithStrangers;

        [JsonProperty("apartment_friendly")]
        public int ApartmentFriendly;

        [JsonProperty("exercise_minutes")]
        public int ExerciseMinutes;

        [JsonProperty("temperament")]
        public string[] Temperament;
    }

    [Serializable]
    public class BreedDataById
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("attributes")]
        public BreedAttributes Attributes;
    }
}
