using System.Threading;
using Cysharp.Threading.Tasks;
using DogBreeds.Model;

namespace DogBreeds.Services
{
    public interface IDogApiClient
    {
        UniTask<BreedListResponse> GetBreedsAsync(CancellationToken cancellationToken);
        UniTask<SingleBreedResponse> GetBreedFactsAsync(string breedId, CancellationToken cancellationToken);
    }
}