using UnityEngine;

namespace DogBreeds.Model
{
    [CreateAssetMenu(fileName = "DogApiConfig", menuName = "TestTask/DogApiConfig")]
    public class DogApiConfig : ScriptableObject
    {
        [Header("API Settings")]
        [Tooltip("Базовый URL Dog API")]
        [SerializeField] private string baseUrl = "https://dogapi.dog/api/v2";

        [Tooltip("Количество пород для отображения")]
        [SerializeField] private int breedLimit = 10;

        [Tooltip("Таймаут запроса (секунды)")]
        [SerializeField] private float requestTimeout = 15f;

        public string BaseUrl => baseUrl;
        public int BreedLimit => breedLimit;
        public float RequestTimeout => requestTimeout;
    }
}