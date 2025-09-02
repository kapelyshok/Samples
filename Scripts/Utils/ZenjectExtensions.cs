using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AtomicApps.Utils
{
    public static class ZenjectExtensions
    {
        public static void InjectGameObjectWithChildren(this DiContainer container, GameObject gameObject)
        {
            MonoBehaviour[] behaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour != null)
                {
                    container.Inject(behaviour);
                }
            }
        }
    }
}
