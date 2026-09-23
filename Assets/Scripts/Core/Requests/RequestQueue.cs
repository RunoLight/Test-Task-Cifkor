using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Requests
{
    /// <summary>
    /// Facade — последовательная очередь HTTP-запросов.
    /// Все запросы выполняются друг за другом, после завершения предыдущего.
    /// Поддерживает добавление, отмену и получение результатов.
    /// </summary>
    public class RequestQueue : MonoBehaviour
    {
        private readonly Queue<IQueuedRequest> _pendingRequests = new();
        private IQueuedRequest _currentRequest;
        private bool _isProcessing;
        private int _requestCounter;

        public int PendingCount => _pendingRequests.Count;
        public bool IsEmpty => _pendingRequests.Count == 0 && _currentRequest == null;

        public async UniTask<T> AddRequest<T>(Func<UniTask<T>> requestFactory, TimeSpan? timeout = null)
        {
            return await AddRequest(ct => requestFactory(), timeout);
        }

        public async UniTask<T> AddRequest<T>(
            Func<CancellationToken, UniTask<T>> requestFactory,
            TimeSpan? timeout = null
        )
        {
            var id = $"req_{++_requestCounter}";
            var tcs = new System.Threading.Tasks.TaskCompletionSource<T>();

            var queuedRequest = new QueuedRequest<T>(id, requestFactory, timeout, tcs);
            _pendingRequests.Enqueue(queuedRequest);

            _ = ProcessLoop();

            return await tcs.Task.AsUniTask();
        }

        /// <summary>
        /// Добавить запрос с callback-ами.
        /// </summary>
        public IRequestHandle AddRequest<T>(
            Func<UniTask<T>> requestFactory,
            Action<T> onSuccess,
            Action<Exception> onError,
            TimeSpan? timeout = null)
        {
            return AddRequest(_ => requestFactory(), onSuccess, onError, timeout);
        }

        public IRequestHandle AddRequest<T>(
            Func<CancellationToken, UniTask<T>> requestFactory,
            Action<T> onSuccess,
            Action<Exception> onError,
            TimeSpan? timeout = null)
        {
            var id = $"req_{++_requestCounter}";

            var queuedRequest = new QueuedRequestWithCallback<T>(id, requestFactory, timeout, onSuccess, onError);
            _pendingRequests.Enqueue(queuedRequest);

            _ = ProcessLoop();
            return queuedRequest;
        }

        public void CancelAll()
        {
            _currentRequest?.Cancel();
            foreach (var request in _pendingRequests)
            {
                request.Cancel();
            }

            _pendingRequests.Clear();
        }

        public void CancelById(string id)
        {
            if (_currentRequest != null && _currentRequest.Id == id)
            {
                _currentRequest.Cancel();
                return;
            }

            var list = new List<IQueuedRequest>(_pendingRequests);
            _pendingRequests.Clear();
            foreach (var request in list)
            {
                if (request.Id == id)
                {
                    request.Cancel();
                    continue;
                }

                _pendingRequests.Enqueue(request);
            }
        }

        private async UniTaskVoid ProcessLoop()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            while (_pendingRequests.Count > 0)
            {
                _currentRequest = _pendingRequests.Dequeue();

                try
                {
                    await _currentRequest.Execute();
                }
                catch (OperationCanceledException)
                {
                    // Cancelled — ignore
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[RequestQueue] Request {_currentRequest.Id} failed: {ex.Message}");
                }
                finally
                {
                    _currentRequest = null;
                }
            }

            _isProcessing = false;
        }

        private void OnDestroy()
        {
            CancelAll();
        }
    }

    public interface IRequestHandle
    {
        string Id { get; }
        bool IsCompleted { get; }
        bool IsCanceled { get; }
        void Cancel();
    }

    internal interface IQueuedRequest : IRequestHandle
    {
        UniTask Execute();
    }

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