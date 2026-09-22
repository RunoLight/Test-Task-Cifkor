using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Clicker.Model
{
    [UsedImplicitly]
    public class ClickerState
    {
        public ReactiveProperty<int> Currency { get; }
        public ReactiveProperty<int> Energy { get; }
        public ClickerConfig Config { get; }
        public int MaxEnergy => Config.MaxEnergy;

        public ClickerState(ClickerConfig config)
        {
            Config = config;
            Currency = new ReactiveProperty<int>(0);
            Energy = new ReactiveProperty<int>(config.MaxEnergy);
        }

        /// <returns> True if click performed, false otherwise. </returns>
        public bool TryClick()
        {
            if (Energy.Value < Config.EnergyCostPerClick)
                return false;

            Energy.Value -= Config.EnergyCostPerClick;
            Currency.Value += Config.RewardPerClick;
            return true;
        }

        /// <returns> True if auto-click performed, false otherwise. </returns>
        public bool TryAutoCollect()
        {
            if (Energy.Value < Config.EnergyCostPerAutoCollect)
                return false;

            Energy.Value -= Config.EnergyCostPerAutoCollect;
            Currency.Value += Config.RewardPerAutoCollect;
            return true;
        }

        public void RegenEnergy()
        {
            Energy.Value = Mathf.Min(Energy.Value + Config.EnergyRegenAmount, Config.MaxEnergy);
        }
    }
}