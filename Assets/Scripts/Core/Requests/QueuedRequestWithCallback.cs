using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Requests
{
    internal sealed class QueuedRequestWithCallback<T> : IQueuedRequest
    {
        public string Id { get; }
        private readonly CancellationTokenSource _cts;
        private readonly Func<CancellationToken, UniTask<T>> _requestFactory;
        private readonly TimeSpan? _timeout;
        private readonly Action<T> _onSuccess;
        private readonly Action<Exception> _onError;

        public bool IsCompleted { get; private set; }
        public bool IsCanceled { get; private set; }

        public QueuedRequestWithCallback(
            string id,
            Func<CancellationToken, UniTask<T>> requestFactory,
            TimeSpan? timeout,
            Action<T> onSuccess,
            Action<Exception> onError)
        {
            Id = id;
            _requestFactory = requestFactory;
            _timeout = timeout;
            _cts = new CancellationTokenSource();
            _onSuccess = onSuccess;
            _onError = onError;
        }

        public async UniTask Execute()
        {
            try
            {
                _cts.Token.ThrowIfCancellationRequested();

                if (_timeout.HasValue)
                    _cts.CancelAfter(_timeout.Value);

                var result = await _requestFactory(_cts.Token);

                if (!IsCanceled)
                    _onSuccess?.Invoke(result);
            }
            catch (OperationCanceledException)
            {
                if (IsCanceled)
                    return;

                _onError?.Invoke(new TimeoutException($"Request '{Id}' timed out."));
            }
            catch (Exception ex)
            {
                _onError?.Invoke(ex);
            }
            finally
            {
                IsCompleted = true;
                _cts.Dispose();
            }
        }

        public void Cancel()
        {
            if (IsCompleted)
                return;

            IsCanceled = true;
            _cts?.Cancel();
        }
    }
}