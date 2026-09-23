using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Navigation;
using UniRx;
using DogBreeds.Model;

namespace DogBreeds.View
{
    public class DogBreedsView : TabView
    {
        [SerializeField] private RectTransform breedListContainer;
        [SerializeField] private BreedItem breedItemPrefab;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private BreedInfoPopup breedInfoPopup;

        private readonly Subject<BreedData> _onBreedClicked = new();
        public IObservable<BreedData> OnBreedClicked => _onBreedClicked.AsObservable();

        private readonly List<BreedItem> _breedItems = new();

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ClearBreedList();
            HideBreedInfoPopup();
        }

        public void ShowBreedList(List<BreedData> breeds)
        {
            ClearBreedList();

            foreach (var breed in breeds)
            {
                var item = CreateBreedItem(breed);
                _breedItems.Add(item);
            }
        }

        private BreedItem CreateBreedItem(BreedData breed)
        {
            var item = Instantiate(breedItemPrefab, breedListContainer);

            item.Setup($"{_breedItems.Count} - {breed.Attributes.Name}", breed.Id);
            item.OnClick
                .Subscribe(_ => _onBreedClicked.OnNext(breed))
                .AddTo(item);

            return item;
        }

        private void ClearBreedList()
        {
            foreach (var item in _breedItems)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }

            _breedItems.Clear();
        }

        public void ShowLoading(bool show)
        {
            loadingIndicator.SetActive(show);
        }

        public void HideLoading()
        {
            ShowLoading(false);
        }

        public void ShowBreedInfoPopup(BreedData breed, string factsText)
        {
            breedInfoPopup.Show(breed.Attributes.Name, factsText);
        }

        public void HideBreedInfoPopup()
        {
            breedInfoPopup.HideInstantly();
        }

        public void ShowError(string message)
        {
            loadingIndicator.SetActive(false);
            Debug.LogError($"[DogBreedsView] Error: {message}");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearBreedList();
            _onBreedClicked?.OnCompleted();
        }
    }
}