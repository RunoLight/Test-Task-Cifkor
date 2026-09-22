using UnityEngine;

namespace Clicker.Model
{
    [CreateAssetMenu(fileName = "ClickerConfig", menuName = "TestTask/ClickerConfig")]
    public class ClickerConfig : ScriptableObject
    {
        [Header("Currency Settings")]
        [Tooltip("Награда за один клик")]
        [SerializeField] private int rewardPerClick = 1;

        [Tooltip("Награда за автосбор")]
        [SerializeField] private int rewardPerAutoCollect = 1;

        [Header("Energy Settings")] 
        [Tooltip("Стоимость одного клика в энергии")] 
        [SerializeField] private int energyCostPerClick = 1;

        [Tooltip("Стоимость автосбора в энергии")] 
        [SerializeField] private int energyCostPerAutoCollect = 1;

        [Tooltip("Максимальное количество энергии")]
        [SerializeField] private int maxEnergy = 1000;

        [Tooltip("Количество энергии, восстанавливаемое за один тик")]
        [SerializeField] private int energyRegenAmount = 10;

        [Tooltip("Интервал восстановления энергии (секунды)")]
        [SerializeField] private float energyRegenInterval = 10f;

        [Header("Auto Collect Settings")] 
        [Tooltip("Интервал автосбора (секунды)")]
        [SerializeField] private float autoCollectInterval = 3f;

        [Header("Button Visual Settings")] [Tooltip("Масштаб кнопки при нажатии")]
        [SerializeField] private float buttonPressScale = 0.9f;

        [Tooltip("Время анимации нажатия кнопки (сек)")]
        [SerializeField] private float buttonPressDuration = 0.1f;

        [Header("Sound Settings")]
        [Tooltip("Громость звука клика (0-1)")] 
        [SerializeField] private float clickSoundVolume = 0.5f;

        public int RewardPerClick => rewardPerClick;
        public int RewardPerAutoCollect => rewardPerAutoCollect;
        public int EnergyCostPerClick => energyCostPerClick;
        public int EnergyCostPerAutoCollect => energyCostPerAutoCollect;
        public int MaxEnergy => maxEnergy;
        public int EnergyRegenAmount => energyRegenAmount;
        public float EnergyRegenInterval => energyRegenInterval;
        public float AutoCollectInterval => autoCollectInterval;
        public float ButtonPressScale => buttonPressScale;
        public float ButtonPressDuration => buttonPressDuration;
        public float ClickSoundVolume => clickSoundVolume;
    }
}