using UnityEngine;

namespace Weather.Model
{
    [CreateAssetMenu(fileName = "WeatherConfig", menuName = "TestTask/WeatherConfig")]
    public class WeatherConfig : ScriptableObject
    {
        [Header("API Settings")]
        [Tooltip("URL API погоды")]
        [SerializeField] private string apiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        [Tooltip("Интервал обновления погоды (секунды)")]
        [SerializeField] private float pollingInterval = 5f;

        [Tooltip("Таймаут запроса (секунды)")]
        [SerializeField] private float requestTimeout = 10f;

        public string ApiUrl => apiUrl;
        public float PollingInterval => pollingInterval;
        public float RequestTimeout => requestTimeout;
    }
}