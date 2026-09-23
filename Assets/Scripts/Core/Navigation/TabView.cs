using UniRx;
using UnityEngine;

namespace Core.Navigation
{
    public abstract class TabView : MonoBehaviour
    {
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

        private readonly ReactiveProperty<bool> _isActive = new(false);

        protected virtual void OnDestroy()
        {
            _isActive.Dispose();
        }

        public void Activate()
        {
            if (_isActive.Value)
                return;

            Debug.Log($"Activated {name}");

            gameObject.SetActive(true);
            _isActive.Value = true;

            OnActivated();
        }

        public void Deactivate()
        {
            if (!_isActive.Value)
                return;

            Debug.Log($"Deactivated {name}");

            _isActive.Value = false;
            OnDeactivated();

            gameObject.SetActive(false);
        }

        protected virtual void OnActivated()
        {
        }

        protected virtual void OnDeactivated()
        {
        }
    }
}