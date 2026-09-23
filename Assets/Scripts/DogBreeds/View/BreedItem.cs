using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using TMPro;

namespace DogBreeds.View
{
    public class BreedItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Button button;

        private readonly ReactiveCommand _onClick = new();
        public IObservable<Unit> OnClick => _onClick.AsObservable();

        public void Setup(string breedName, string breedId)
        {
            gameObject.SetActive(true);
            nameText.text = breedName;
        }

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
            _onClick.Dispose();
        }

        private void AnimatePress()
        {
            backgroundImage.DOKill();
            backgroundImage.DOFade(0.7f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    backgroundImage.DOFade(1f, 0.1f)
                        .SetEase(Ease.InQuad);
                });
        }

        private void OnButtonClick()
        {
            AnimatePress();
            _onClick.Execute(Unit.Default);
        }
    }
}