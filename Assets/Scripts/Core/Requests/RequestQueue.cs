using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Requests
{
    [UsedImplicitly]
    public class RequestQueue : IDisposable, IRequestQueue
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

        public void Dispose()
        {
            CancelAll();
        }
    }
}