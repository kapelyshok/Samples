using System;
using UnityEngine;
using Zenject;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField]
        private GameBootstrapper gameBootstrapper;

        private DiContainer _container;

        [Inject]
        private void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;

            if (FindObjectOfType(typeof(GameBootstrapper)) == null)
            {
                _container.InstantiatePrefab(gameBootstrapper);
            }
        }
    }
}
