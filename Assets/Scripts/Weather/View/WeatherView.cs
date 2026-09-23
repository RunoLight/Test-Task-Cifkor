using Core.Navigation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Weather.View
{
    public sealed class WeatherView : TabView
    {
        [SerializeField] private Image weatherIcon;
        [SerializeField] private TMP_Text weatherText;
        [SerializeField] private GameObject loadingIndicator;

        public void UpdateWeather(string statusText)
        {
            weatherText.text = $"Today - {statusText}";
        }

        public void SetWeatherIcon(Sprite sprite)
        {
            weatherIcon.color = sprite == null ? Color.clear : Color.white;
            weatherIcon.sprite = sprite;
        }

        public void ShowLoading(bool show)
        {
            loadingIndicator.SetActive(show);
        }

        public void ShowError(string message)
        {
            weatherText.text = $"Error: {message}";
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            SetWeatherIcon(null);
            weatherText.text = "";
            ShowLoading(false);
        }
    }
}