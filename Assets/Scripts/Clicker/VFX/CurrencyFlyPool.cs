using UnityEngine;
using UnityEngine.Pool;

namespace Clicker.VFX
{
    public class CurrencyFlyPool : MonoBehaviour, ICurrencyFlySpawner
    {
        [SerializeField] private CurrencyFlyAnimation currencyFlyPrefab;
        [SerializeField] private int poolSize = 10;
        [SerializeField] private float spread = 40f;

        private ObjectPool<ICurrencyAnimation> _pool;

        void ICurrencyFlySpawner.Spawn(Vector2 position)
        {
            _pool.Get()
                .Play(position + Random.insideUnitCircle * spread, onComplete: Release);
        }

        private void Awake()
        {
            _pool = new ObjectPool<ICurrencyAnimation>(
                Create, OnGet, OnRelease, OnDestroyElement,
                collectionCheck: true,
                defaultCapacity: poolSize,
                maxSize: poolSize * 4
            );
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        private ICurrencyAnimation Create()
        {
            return Instantiate(currencyFlyPrefab, transform);
        }

        private static void OnGet(ICurrencyAnimation anim)
        {
            anim.Enable();
        }

        private static void OnRelease(ICurrencyAnimation anim)
        {
            anim.Disable();
        }

        private static void OnDestroyElement(ICurrencyAnimation anim)
        {
            anim.Dispose();
        }

        private void Release(ICurrencyAnimation anim)
        {
            _pool.Release(anim);
        }
    }
}