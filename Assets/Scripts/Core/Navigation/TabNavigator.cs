using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Core.Navigation
{
    /// <summary>
    /// Mediator - координирует переключение вкладок.
    /// </summary>
    public class TabNavigator : MonoBehaviour
    {
        [SerializeField] private List<TabView> tabs = new();
        [SerializeField] private int activeTabIndex;

        public int ActiveTabIndex => activeTabIndex;
        public TabView ActiveTab => tabs.Count > activeTabIndex ? tabs[activeTabIndex] : null;

        private readonly Subject<int> _tabChanged = new();
        public IObservable<int> OnTabChanged => _tabChanged.AsObservable();

        private void Awake()
        {
            if (tabs.Count == 0)
            {
                Debug.LogWarning("[TabNavigator] No tabs assigned!");
                return;
            }

            foreach (var tab in tabs)
            {
                tab.Deactivate();
            }

            if (tabs.Count > 0)
            {
                SwitchTo(0, sendEvent: false);
            }
        }
        
        public void SwitchTo(int tabIndex, bool sendEvent = true)
        {
            if (tabIndex < 0 || tabIndex >= tabs.Count)
            {
                Debug.LogError($"[TabNavigator] Invalid tab index: {tabIndex}");
                return;
            }

            if (tabs[activeTabIndex] != null)
            {
                tabs[activeTabIndex].Deactivate();
            }

            activeTabIndex = tabIndex;

            if (tabs[activeTabIndex] != null)
            {
                tabs[activeTabIndex].Activate();
            }

            if (sendEvent)
            {
                _tabChanged.OnNext(activeTabIndex);
            }
        }
        
        public void SwitchTo<T>() where T : TabView
        {
            var index = tabs.FindIndex(t => t is T);
            if (index >= 0)
            {
                SwitchTo(index);
            }
            else
            {
                Debug.LogError($"[TabNavigator] Tab of type {typeof(T).Name} not found!");
            }
        }

        private void OnDestroy()
        {
            _tabChanged?.OnCompleted();
        }
    }
}