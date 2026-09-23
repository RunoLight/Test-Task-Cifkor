using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Requests
{
    /// <summary>
    /// Facade — последовательная очередь HTTP-запросов.
    /// Все запросы выполняются друг за другом, после завершения предыдущего.
    /// Поддерживает добавление, отмену и получение результатов.
    /// </summary>
    public interface IRequestQueue
    {
        int PendingCount { get; }
        bool IsEmpty { get; }
        UniTask<T> AddRequest<T>(Func<UniTask<T>> requestFactory, TimeSpan? timeout = null);

        UniTask<T> AddRequest<T>(
            Func<CancellationToken, UniTask<T>> requestFactory,
            TimeSpan? timeout = null
        );

        IRequestHandle AddRequest<T>(
            Func<UniTask<T>> requestFactory,
            Action<T> onSuccess,
            Action<Exception> onError,
            TimeSpan? timeout = null);

        IRequestHandle AddRequest<T>(
            Func<CancellationToken, UniTask<T>> requestFactory,
            Action<T> onSuccess,
            Action<Exception> onError,
            TimeSpan? timeout = null);

        void CancelAll();
        void CancelById(string id);
    }
}