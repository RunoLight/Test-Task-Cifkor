using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace DogBreeds.View
{
    public class BreedInfoPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text breedNameText;
        [SerializeField] private TMP_Text factsText;
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Show(string breedName, string facts)
        {
            breedNameText.text = breedName;
            factsText.text = facts;

            canvasGroup.DOKill();
            contentPanel.DOKill();
            
            gameObject.SetActive(true);

            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);

            contentPanel.localScale = Vector3.one * 0.9f;
            contentPanel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            canvasGroup.DOKill();
            contentPanel.DOKill();
            
            canvasGroup
                .DOFade(0f, 0.2f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }   
        
        public void HideInstantly()
        {
            canvasGroup.DOKill();
            contentPanel.DOKill();

            canvasGroup.alpha = 0f;
            contentPanel.localScale = Vector3.one * 0.9f;
            
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(Hide);
        }
    }
}