using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Core.Navigation
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private TabNavigator tabNavigator;
        [SerializeField] private Button[] tabButtons;

        private void Awake()
        {
            if (tabButtons != null)
            {
                for (int i = 0; i < tabButtons.Length; i++)
                {
                    if (tabButtons[i] != null)
                    {
                        var i1 = i;
                        tabButtons[i].onClick.AddListener(() => tabNavigator.SwitchTo(i1));
                    }
                }
            }

            tabNavigator.OnTabChanged.Subscribe(UpdateActiveButton)
                .AddTo(this);
        }

        private void Start()
        {
            UpdateActiveButton(tabNavigator.ActiveTabIndex);
        }

        private void UpdateActiveButton(int activeIndex)
        {
            if (tabButtons == null)
                return;

            for (int i = 0; i < tabButtons.Length; i++)
            {
                var isActive = i == activeIndex;
                tabButtons[i].interactable = !isActive;
            }
        }
    }
}