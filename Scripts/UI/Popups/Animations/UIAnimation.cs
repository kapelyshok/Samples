using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.UI.Popups
{
    public abstract class UIAnimation : ScriptableObject
    {
        [SerializeField]
        protected float duration = 1f;
        [SerializeField]
        protected Ease ease = Ease.Linear;
        
        public event Action OnAnimationFinished;
        
        public virtual async UniTask ActivateAsync(GameObject target, bool inAnimation, Action onCompletedCallback = null)
        {
            if (target == null)
            {
                Debug.LogError("Can't perform UI animation. Target is null");
                return;
            }
        }

        protected void FireAnimationFinished()
        {
            OnAnimationFinished?.Invoke();
        }
    }
}