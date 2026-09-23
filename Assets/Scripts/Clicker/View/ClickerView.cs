using System;
using Clicker.Model;
using UnityEngine;
using UnityEngine.UI;
using Core.Navigation;
using DG.Tweening;
using TMPro;
using UniRx;
using Zenject;

namespace Clicker.View
{
    public class ClickerView : TabView
    {
        [SerializeField] private Button clickButton;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text energyText;
        [SerializeField] private Image buttonImage;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickSound;

        [Inject] private ClickerState _state;

        private readonly Subject<Unit> _onClick = new();

        public IObservable<Unit> OnClick => _onClick.AsObservable();

        private Sequence _pressSequence;
        private Vector3 _baseScale;

        private void Awake()
        {
            clickButton.onClick.AddListener(OnClickButton);

            _baseScale = buttonImage.transform.localScale;

            float scale = _state.Config.ButtonPressScale;
            float duration = _state.Config.ButtonPressDuration;

            _pressSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .SetLink(gameObject);

            _pressSequence
                .Append(
                    buttonImage.transform
                        .DOScale(_baseScale * scale, duration)
                        .SetEase(Ease.OutQuad)
                )
                .Append(
                    buttonImage.transform
                        .DOScale(_baseScale, duration)
                        .SetEase(Ease.InQuad)
                );
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            clickButton.onClick.RemoveListener(OnClickButton);

            _onClick?.OnCompleted();
            _onClick?.Dispose();

            _pressSequence?.Kill();
        }

        private void OnClickButton()
        {
            _onClick.OnNext(Unit.Default);
        }

        public void UpdateCurrency(int currency)
        {
            currencyText.text = $"{currency}";
        }

        public void UpdateEnergy(int energy, int maxEnergy)
        {
            energyText.text = $"{energy}/{maxEnergy}";
        }

        public void SetButtonEnabled(bool isEnabled)
        {
            clickButton.interactable = isEnabled;
        }

        public void AnimateButtonPress()
        {
            _pressSequence.Restart();
        }

        public void PlayClickSound(float volume)
        {
            if (audioSource == null || clickSound == null)
                return;

            audioSource.PlayOneShot(clickSound, volume);
        }

        public Vector2 GetButtonPosition()
        {
            if (clickButton != null)
            {
                return clickButton.transform.position;
            }

            return Vector2.zero;
        }

        private void OnValidate()
        {
            if (clickButton == null)
                Debug.LogError($"{nameof(ClickerView)}: Click Button is not assigned.", this);

            if (currencyText == null)
                Debug.LogError($"{nameof(ClickerView)}: Currency Text is not assigned.", this);

            if (energyText == null)
                Debug.LogError($"{nameof(ClickerView)}: Energy Text is not assigned.", this);

            if (buttonImage == null)
                Debug.LogError($"{nameof(ClickerView)}: Button Image is not assigned.", this);

            if (audioSource == null)
                Debug.LogError($"{nameof(ClickerView)}: Audio Source is not assigned.", this);

            if (clickSound == null)
                Debug.LogError($"{nameof(ClickerView)}: Click Sound is not assigned.", this);
        }
    }
}