using System;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicApps.UI.Mechanics
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private RectTransform activeState;
        [SerializeField] private RectTransform inactiveState;

        [SerializeField] private Image activeIcon;
        [SerializeField] private Image inActiveIcon;

        public bool IsActive { get; private set; }

        public event Action<bool> OnStateChanged;

        private void Awake()
        {
            button.onClick.AddListener(ChangeState);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(ChangeState);
        }

        public void ChangeState(bool state)
        {
            IsActive = state;
            activeState.gameObject.SetActive(IsActive);
            inactiveState.gameObject.SetActive(!IsActive);
            activeIcon.gameObject.SetActive(IsActive);
            inActiveIcon.gameObject.SetActive(!IsActive);
            OnStateChanged?.Invoke(IsActive);
        }
        
        public void ChangeState()
        {
            IsActive = !IsActive;
            ChangeState(IsActive);
        }
    }
}
