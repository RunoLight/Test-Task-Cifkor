using Cysharp.Threading.Tasks;

namespace Core.Requests
{
    internal interface IQueuedRequest : IRequestHandle
    {
        UniTask Execute();
    }
}