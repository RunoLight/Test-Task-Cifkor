using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Requests
{
    internal sealed class QueuedRequest<T> : IQueuedRequest
    {
        public string Id { get; }
        private readonly CancellationTokenSource _cts;
        private readonly Func<CancellationToken, UniTask<T>> _requestFactory;
        private readonly TimeSpan? _timeout;
        private readonly System.Threading.Tasks.TaskCompletionSource<T> _tcs;

        public bool IsCompleted { get; private set; }
        public bool IsCanceled { get; private set; }

        public QueuedRequest(
            string id,
            Func<CancellationToken, UniTask<T>> requestFactory,
            TimeSpan? timeout,
            System.Threading.Tasks.TaskCompletionSource<T> tcs)
        {
            Id = id;
            _requestFactory = requestFactory;
            _timeout = timeout;
            _cts = new CancellationTokenSource();
            _tcs = tcs;
        }

        public async UniTask Execute()
        {
            try
            {
                _cts.Token.ThrowIfCancellationRequested();

                if (_timeout.HasValue)
                {
                    _cts.CancelAfter(_timeout.Value);
                }

                var result = await _requestFactory(_cts.Token);

                if (!_tcs.Task.IsCompleted)
                {
                    _tcs.SetResult(result);
                }
            }
            catch (OperationCanceledException)
            {
                IsCanceled = true;
                if (!_tcs.Task.IsCompleted)
                {
                    _tcs.SetCanceled();
                }
            }
            catch (Exception ex)
            {
                if (!_tcs.Task.IsCompleted)
                {
                    _tcs.SetException(ex);
                }
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
            if (!_tcs.Task.IsCompleted)
            {
                _tcs.SetCanceled();
            }
        }
    }
}