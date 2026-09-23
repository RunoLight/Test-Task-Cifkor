using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Requests;
using DogBreeds.Formatters;
using UniRx;
using DogBreeds.Model;
using DogBreeds.Services;
using DogBreeds.View;
using JetBrains.Annotations;
using Zenject;

namespace DogBreeds.Controller
{
    [UsedImplicitly]
    public class DogBreedsController : IInitializable, IDisposable
    {
        [Inject] private RequestQueue _requestQueue;
        [Inject] private IDogApiClient _apiClient;
        [Inject] private DogApiConfig _apiConfig;
        [Inject] private DogBreedsView _view;

        private List<BreedData> _currentBreeds = new();
        private readonly CompositeDisposable _disposables = new();
        private IRequestHandle _activeBreedListRequest;
        private IRequestHandle _activeFactsRequest;
        private bool _isFetchingBreeds;

        public void Initialize()
        {
            _view.OnBreedClicked
                .Subscribe(OnBreedClicked)
                .AddTo(_disposables);

            _view.IsActive
                .DistinctUntilChanged()
                .Subscribe(isActive =>
                {
                    if (isActive)
                    {
                        FetchBreeds();
                    }
                    else
                    {
                        CancelAllRequests();
                    }
                })
                .AddTo(_disposables);
        }

        private void FetchBreeds()
        {
            if (_isFetchingBreeds)
                return;

            _isFetchingBreeds = true;

            ShowLoading(true);

            _activeBreedListRequest = _requestQueue.AddRequest(
                cancellationToken => _apiClient.GetBreedsAsync(cancellationToken),
                OnBreedsFetched,
                OnBreedsError,
                timeout: TimeSpan.FromSeconds(_apiConfig.RequestTimeout)
            );
        }

        private void OnBreedsFetched(BreedListResponse response)
        {
            _activeBreedListRequest = null;
            _isFetchingBreeds = false;
            ShowLoading(false);

            _currentBreeds = response.Data.ToList();
            _view.ShowBreedList(_currentBreeds);
        }

        private void OnBreedsError(Exception error)
        {
            _activeBreedListRequest = null;
            _isFetchingBreeds = false;
            ShowLoading(false);
            _view.ShowError(error.Message);
        }

        private void OnBreedClicked(BreedData breed)
        {
            CancelFactsRequest();
            FetchBreedFacts(breed);
        }

        private void FetchBreedFacts(BreedData breed)
        {
            _view.HideBreedInfoPopup();
            _view.ShowLoading(true);

            _activeFactsRequest = _requestQueue.AddRequest(
                cancellationToken => _apiClient.GetBreedFactsAsync(breed.Id, cancellationToken),
                result => OnBreedFactsFetched(breed, result),
                error => OnBreedFactsError(breed, error),
                timeout: TimeSpan.FromSeconds(_apiConfig.RequestTimeout)
            );
        }

        private void OnBreedFactsFetched(BreedData breed, SingleBreedResponse response)
        {
            _activeFactsRequest = null;

            var reviewText = BreedDescriptionFormatter.GenerateRussianReview(response);
            _view.ShowLoading(false);
            _view.ShowBreedInfoPopup(breed, reviewText);
        }

        private void OnBreedFactsError(BreedData breed, Exception error)
        {
            _activeFactsRequest = null;
            _view.ShowLoading(false);
            _view.ShowError($"Failed to load facts for {breed.Id}: {error.Message}");
        }

        private void CancelAllRequests()
        {
            CancelBreedListRequest();
            CancelFactsRequest();
        }

        private void ShowLoading(bool show)
        {
            _view.ShowLoading(show);
        }

        private void CancelBreedListRequest()
        {
            if (_activeBreedListRequest == null)
                return;

            if (!_activeBreedListRequest.IsCompleted)
            {
                _requestQueue.CancelById(_activeBreedListRequest.Id);
            }

            _activeBreedListRequest = null;
            _isFetchingBreeds = false;
            _view.ShowLoading(false);
        }

        private void CancelFactsRequest()
        {
            if (_activeFactsRequest == null)
                return;

            if (!_activeFactsRequest.IsCompleted)
            {
                _requestQueue.CancelById(_activeFactsRequest.Id);
            }

            _activeFactsRequest = null;
            _view.ShowLoading(false);
        }

        public void Dispose()
        {
            CancelAllRequests();
            _disposables.Dispose();
        }
    }
}