using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using Clicker.Model;
using Clicker.View;
using Clicker.VFX;
using JetBrains.Annotations;
using Zenject;

namespace Clicker.Controller
{
    [UsedImplicitly]
    public class ClickerController : IInitializable, IDisposable
    {
        private readonly ClickerState _state;
        private readonly ClickerView _view;
        private readonly IParticleSpawner _particlePool;
        private readonly ICurrencyFlySpawner _currencyFlySpawner;

        private readonly CompositeDisposable _disposables = new();
        private CancellationTokenSource _tabCts;

        public ClickerController(
            ClickerState state,
            ClickerView view,
            IParticleSpawner particlePool,
            ICurrencyFlySpawner currencyFlySpawner)
        {
            _state = state;
            _view = view;
            _particlePool = particlePool;
            _currencyFlySpawner = currencyFlySpawner;
        }

        public void Initialize()
        {
            _state.Currency
                .Subscribe(UpdateCurrencyView)
                .AddTo(_disposables);

            _state.Energy
                .Subscribe(UpdateEnergyView)
                .AddTo(_disposables);

            _view.OnClick
                .Subscribe(OnButtonClick)
                .AddTo(_disposables);

            _view.IsActive
                .DistinctUntilChanged()
                .Subscribe(OnViewIsActiveChanged)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            StopLoops();
            _disposables.Dispose();
        }

        private void OnViewIsActiveChanged(bool isActive)
        {
            if (isActive)
            {
                StartLoops();
            }
            else
            {
                StopLoops();
            }
        }

        private void OnButtonClick(Unit unit)
        {
            if (!_state.TryClick())
                return;

            PlayClickFeedback();
        }

        private void StartLoops()
        {
            StopLoops();

            _tabCts = new CancellationTokenSource();
            var token = _tabCts.Token;

            AutoCollectLoopAsync(token).Forget();
            EnergyRegenLoopAsync(token).Forget();
        }

        private void StopLoops()
        {
            if (_tabCts == null)
                return;

            _tabCts.Cancel();
            _tabCts.Dispose();
            _tabCts = null;
        }

        private async UniTask AutoCollectLoopAsync(CancellationToken token)
        {
            var delayMs = Mathf.RoundToInt(_state.Config.AutoCollectInterval * 1000f);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(delayMs, cancellationToken: token);

                    if (_state.TryAutoCollect())
                        PlayClickFeedback();
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальное поведение при деактивации вкладки — просто выходим из метода
            }
        }

        private async UniTask EnergyRegenLoopAsync(CancellationToken token)
        {
            var delayMs = Mathf.RoundToInt(_state.Config.EnergyRegenInterval * 1000f);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(delayMs, cancellationToken: token);
                    _state.RegenEnergy();
                }
            }
            catch (OperationCanceledException)
            {
                // Исключение перехватывается, предотвращая спам в консоль Unity при отмене
            }
        }

        private void UpdateCurrencyView(int currency)
        {
            _view.UpdateCurrency(currency);
        }

        private void UpdateEnergyView(int energy)
        {
            _view.UpdateEnergy(energy, _state.MaxEnergy);
            _view.SetButtonEnabled(energy >= _state.Config.EnergyCostPerClick);
        }

        private void PlayClickFeedback()
        {
            _view.AnimateButtonPress();
            _view.PlayClickSound(_state.Config.ClickSoundVolume);

            var clickPosition = _view.GetButtonPosition();
            _particlePool.Spawn(clickPosition);
            _currencyFlySpawner.Spawn(clickPosition);
        }
    }
}