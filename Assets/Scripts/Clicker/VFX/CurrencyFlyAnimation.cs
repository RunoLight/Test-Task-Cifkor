using System;
using DG.Tweening;
using UnityEngine;

namespace Clicker.VFX
{
    /// <summary>
    /// Reusable currency flight animation for pooled objects.
    /// Creates DOTween objects only once and reuses the same Sequence.
    /// </summary>
    public sealed class CurrencyFlyAnimation : MonoBehaviour, ICurrencyAnimation
    {
        [SerializeField, Min(0.01f)] private float duration = 0.5f;
        [SerializeField, Min(0f)] private float flyDistance = 500f;
        [SerializeField, Min(0f)] private float arcHeight = 100f;
        [SerializeField, Min(0f)] private float horizontalSpread = 80f;

        [SerializeField, Min(0f)] private float startScale = 0.75f;
        [SerializeField, Min(0f)] private float peakScale = 1.1f;
        [SerializeField, Min(0f)] private float endScale = 0.8f;

        [SerializeField, Range(0f, 1f)] private float fadeStart = 0.7f;

        [SerializeField] private CanvasGroup canvasGroup;

        private Sequence _sequence;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector3 _controlPoint;

        private Vector3 _baseScale;

        private float _progress;

        private Action<ICurrencyAnimation> _onComplete;

        #region ICurrencyAnimation

        public void Play(Vector2 startPosition, Action<ICurrencyAnimation> onComplete)
        {
            _onComplete = onComplete;

            _startPosition = startPosition;
            _endPosition = _startPosition + Vector3.up * flyDistance;

            _controlPoint = Vector3.Lerp(_startPosition, _endPosition, 0.5f);
            _controlPoint += Vector3.up * arcHeight;
            _controlPoint += Vector3.right * UnityEngine.Random.Range(-horizontalSpread, horizontalSpread);

            ResetVisualState();
            _progress = 0f;
            _sequence.Restart();
        }

        public void Stop()
        {
            _sequence.Pause();
            _onComplete = null;
            ResetVisualState();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            enabled = true;
        }

        public void Disable()
        {
            Stop();
            enabled = false;
            gameObject.SetActive(false);
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        #endregion

        private void Awake()
        {
            _baseScale = transform.localScale;
            CreateSequence();
        }

        private void OnDestroy()
        {
            _onComplete = null;
            _sequence?.Kill();
        }

        private void CreateSequence()
        {
            _sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .SetLink(gameObject);

            // One reusable progress tween.
            _sequence.Append(
                DOTween.To(
                        () => _progress,
                        value =>
                        {
                            _progress = value;
                            UpdateTransform(value);
                        },
                        1f,
                        duration
                    )
                    .SetEase(Ease.OutCubic)
            );

            _sequence.OnComplete(Complete);
        }

        private void ResetVisualState()
        {
            transform.localScale = _baseScale;
            canvasGroup.alpha = 1f;
        }

        private void Complete()
        {
            var callback = _onComplete;
            _onComplete = null;

            callback?.Invoke(this);
        }

        #region Animation

        private void UpdateTransform(float t)
        {
            var position = QuadraticBezier(
                _startPosition,
                _controlPoint,
                _endPosition,
                t
            );

            transform.position = position;

            var scaleT = Mathf.Clamp01(t / 0.25f);

            float scale;

            if (t < 0.25f)
            {
                scale = Mathf.Lerp(
                    startScale,
                    peakScale,
                    EaseOutBack(scaleT)
                );
            }
            else
            {
                var releaseT = Mathf.InverseLerp(
                    0.25f,
                    1f,
                    t
                );

                scale = Mathf.Lerp(
                    peakScale,
                    endScale,
                    releaseT
                );
            }

            transform.localScale = _baseScale * scale;

            var alphaT = Mathf.InverseLerp(
                fadeStart,
                1f,
                t
            );

            canvasGroup.alpha = 1f - alphaT;
        }

        private static Vector3 QuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t)
        {
            var oneMinusT = 1f - t;

            return oneMinusT * oneMinusT * start +
                   2f * oneMinusT * t * control +
                   t * t * end;
        }

        private static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;

            var value = t - 1f;

            return 1f + (overshoot + 1f) * value * value * value + overshoot * value * value;
        }

        #endregion
    }
}