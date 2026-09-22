using System;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;

namespace Clicker.VFX
{
    public sealed class PooledParticle : MonoBehaviour
    {
        [SerializeField] private UIParticle particle;
        [SerializeField] private float lifetime = 1f;

        private Tween _lifetimeTween;
        private Action<PooledParticle> _onComplete;

        public void Play(Vector2 position, Action<PooledParticle> onComplete)
        {
            _onComplete = onComplete;

            transform.position = position;

            particle.gameObject.SetActive(true);
            particle.Play();

            _lifetimeTween.Restart();
        }

        public void Stop()
        {
            _lifetimeTween?.Pause();

            _onComplete = null;

            particle.Stop();
            particle.Clear();
        }

        private void Awake()
        {
            _lifetimeTween = DOVirtual.DelayedCall(lifetime, Complete);

            _lifetimeTween
                .SetAutoKill(false)
                .Pause()
                .SetLink(gameObject);
        }

        private void OnDestroy()
        {
            _onComplete = null;
            _lifetimeTween?.Kill();
        }

        private void Complete()
        {
            var callback = _onComplete;
            _onComplete = null;

            callback?.Invoke(this);
        }
    }
}