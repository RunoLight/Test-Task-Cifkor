using System;
using System.Threading;
using Core;
using Core.Requests;
using Cysharp.Threading.Tasks;
using UniRx;
using JetBrains.Annotations;
using Weather.Model;
using Weather.Services;
using Weather.Services.WeatherIconLoader;
using Weather.View;
using Zenject;

namespace Weather.Controller
{
    [UsedImplicitly]
    public sealed class WeatherController : IInitializable, IDisposable
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IWeatherApiClient _apiClient;
        private readonly WeatherConfig _config;
        private readonly WeatherView _view;
        private readonly IWeatherIconLoader _iconLoader;

        private readonly CompositeDisposable _disposables = new();

        private CancellationTokenSource _lifetimeCts;
        private CancellationTokenSource _pollingCts;

        private IRequestHandle _activeRequest;

        public WeatherController(
            IRequestQueue requestQueue,
            IWeatherApiClient apiClient,
            WeatherConfig config,
            WeatherView view,
            IWeatherIconLoader iconLoader)
        {
            _requestQueue = requestQueue;
            _apiClient = apiClient;
            _config = config;
            _view = view;
            _iconLoader = iconLoader;
        }

        public void Initialize()
        {
            _lifetimeCts = new CancellationTokenSource();

            _view.IsActive
                .DistinctUntilChanged()
                .Subscribe(OnActiveChanged)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            StopPolling();

            _lifetimeCts?.Cancel();
            _lifetimeCts?.Dispose();
            _lifetimeCts = null;

            _disposables.Dispose();
        }

        private void OnActiveChanged(bool isActive)
        {
            if (isActive)
                StartPolling();
            else
                StopPolling();
        }

        private void StartPolling()
        {
            if (_pollingCts != null)
                return;

            _pollingCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token);
            PollAsync(_pollingCts.Token).Forget();
        }

        private void StopPolling()
        {
            if (_pollingCts == null)
                return;

            var cts = _pollingCts;
            _pollingCts = null;

            cts.Cancel();
            cts.Dispose();

            CancelWeatherRequest();

            _view.ShowLoading(false);
        }

        private async UniTaskVoid PollAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    FetchWeather(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(_config.PollingInterval), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void FetchWeather(CancellationToken pollingToken)
        {
            if (_activeRequest is { IsCompleted: false })
                return;

            _view.ShowLoading(true);

            _activeRequest = _requestQueue.AddRequest(
                cancellationToken => _apiClient.GetForecastAsync(cancellationToken),
                forecast => OnWeatherFetched(forecast, pollingToken),
                OnWeatherError,
                timeout: TimeSpan.FromSeconds(_config.RequestTimeout)
            );
        }

        private void OnWeatherFetched(WeatherForecast forecast, CancellationToken pollingToken)
        {
            if (pollingToken.IsCancellationRequested)
                return;

            _activeRequest = null;
            _view.ShowLoading(false);

            var periods = forecast?.Properties?.Periods;

            if (periods == null || periods.Length == 0)
                return;

            var today = periods[0];

            _view.UpdateWeather($"{today.Temperature}{today.TemperatureUnit}");

            if (!string.IsNullOrEmpty(today.Icon))
            {
                LoadIconAsync(today.Icon, pollingToken).Forget();
            }
        }

        private async UniTaskVoid LoadIconAsync(string url, CancellationToken token)
        {
            try
            {
                var sprite = await _iconLoader.LoadAsync(url, token);

                token.ThrowIfCancellationRequested();

                _view.SetWeatherIcon(sprite);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                // Icon failure must not affect weather data.
            }
        }

        private void OnWeatherError(Exception error)
        {
            _activeRequest = null;
            _view.ShowLoading(false);
            _view.ShowError(error.Message);
        }

        private void CancelWeatherRequest()
        {
            var request = _activeRequest;

            if (request == null)
                return;

            _activeRequest = null;

            if (!request.IsCompleted)
                request.Cancel();
        }
    }
}