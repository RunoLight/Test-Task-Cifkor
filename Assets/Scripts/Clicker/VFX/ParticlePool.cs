using UnityEngine;
using UnityEngine.Pool;

namespace Clicker.VFX
{
    public sealed class ParticlePool : MonoBehaviour, IParticleSpawner
    {
        [SerializeField] private PooledParticle particlePrefab;
        [SerializeField] private int poolSize = 20;

        private ObjectPool<PooledParticle> _pool;

        void IParticleSpawner.Spawn(Vector2 position)
        {
            _pool.Get().Play(position, Release);
        }

        private void Awake()
        {
            _pool = new ObjectPool<PooledParticle>(
                Create, OnGet, OnRelease, OnDestroyParticle,
                collectionCheck: true,
                defaultCapacity: poolSize,
                maxSize: poolSize * 2
            );
        }

        private void OnDestroy()
        {
            _pool?.Dispose();
        }

        private PooledParticle Create()
        {
            return Instantiate(particlePrefab, transform);
        }

        private static void OnGet(PooledParticle particle)
        {
            particle.gameObject.SetActive(true);
        }

        private static void OnRelease(PooledParticle particle)
        {
            particle.Stop();
            particle.gameObject.SetActive(false);
        }

        private static void OnDestroyParticle(PooledParticle particle)
        {
            particle.Stop();
            Destroy(particle.gameObject);
        }

        private void Release(PooledParticle particle)
        {
            _pool.Release(particle);
        }
    }
}