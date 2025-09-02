using System;
using System.Collections.Generic;
using AtomicApps.Tools;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AtomicApps.Pooling
{
    public class ObjectPool : MonoBehaviour, IObjectPool
    {
        [HelpBox(
            @"SETUP INSTRUCTION
1. Every object you want pool to work with should implement IPoolable. (Now we have PoolableMonoBehaviour and PoolableParticle for convenience)
2. Use GetObjectAsync to get object from pool, if pool is empty it will create an object and return it.
3. Use WarmupPoolAsync to prepare several objects for the future.", HelpBoxMessageType.Info)]
        public bool readMe;
        
        [Space]
        [Header("Canvas Templates")]
        [SerializeField] private Canvas canvasTemplate;
        [SerializeField] private CanvasScaler canvasScalerTemplate;
        [SerializeField] private GraphicRaycaster graphicRaycasterTemplate;
        
        [Space]
        private readonly Dictionary<Component, PoolTask> _activePoolTasks = new();
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;

        private void Awake()
        {
            _signalBus.Subscribe<SceneContextReadySignal>(TryUpdateDiContainer);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<SceneContextReadySignal>(TryUpdateDiContainer);
        }

        /// <summary>
        /// Alert!
        /// This method allows your objects to have dependencies from SceneContext of current scene. But it can be tricky,
        /// so I would recommend to make unique pools for each scene if you want to implement such logic,
        /// or clear all pooling tasks when you change the scene.
        /// It's because when you leave the scene all the poolable objects will stay in this pool and some dependencies can be lost.
        /// With great power comes great responsibility. Use wisely.
        /// </summary>
        private void TryUpdateDiContainer(SceneContextReadySignal signal)
        {
            SceneContext currentSceneContext = signal.SceneContext;
            
            if (currentSceneContext != null)
            {
                _diContainer = currentSceneContext.Container;

                foreach (var poolTask in _activePoolTasks)
                {
                    poolTask.Value.UpdateDIContainer(_diContainer);
                }
            }
            else
            {
                Debug.LogWarning("No SceneContext found in the loaded scene.");
            }
        }

        public async UniTask<T> GetObjectAsync<T>(T prefab) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                Debug.LogError("Object pool can't process request because prefab is null!");
                return null;
            }
            
            if (!_activePoolTasks.TryGetValue(prefab, out var poolTask))
                AddTaskToPool(prefab, out poolTask);

            return await poolTask.GetFreeObject(prefab);
        }
        
        public async UniTask WarmupPoolAsync<T>(T prefab, int count) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                Debug.LogError("Object pool can't process request because prefab is null!");
                return;
            }
            
            if (!_activePoolTasks.TryGetValue(prefab, out var poolTask))
                AddTaskToPool(prefab, out poolTask);

            List<UniTask> tasks = new();

            for (int i = 0; i < count; i++)
            {
                tasks.Add(poolTask.PrepareObject(prefab));
            }

            await UniTask.WhenAll(tasks);
        }

        public int GetReadyObjectsCount<T>(T prefab) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                Debug.LogError("Object pool can't process request because prefab is null!");
                return 0;
            }

            if (!_activePoolTasks.TryGetValue(prefab, out var poolTask))
            {
                return 0;
            }

            return poolTask.GetFreeObjectsCount();
        }

        private void AddTaskToPool<T>(T prefab, out PoolTask poolTask) where T : Component, IPoolable
        {
            GameObject taskContainer;
            if (prefab.GetComponent<RectTransform>() != null)
            {
                taskContainer = new GameObject($"{prefab.name}_pool")
                {
                    transform = { parent = transform }
                };
                var newCanvas = taskContainer.AddComponent<Canvas>();
                CopyCanvasSettings(canvasTemplate, newCanvas);
                var newCanvasScaler = taskContainer.AddComponent<CanvasScaler>();
                CopyCanvasScalerSettings(canvasScalerTemplate, newCanvasScaler);
                var newGraphicRaycaster = taskContainer.AddComponent<GraphicRaycaster>();
                CopyRaycasterSettings(graphicRaycasterTemplate, newGraphicRaycaster);
            }
            else
            {
                taskContainer = new GameObject($"{prefab.name}_pool")
                {
                    transform = { parent = transform }
                };
            }
            

            poolTask = _diContainer.Instantiate<PoolTask>(new object[] { taskContainer.transform });
            _activePoolTasks.Add(prefab, poolTask);
        }
        
        private void CopyCanvasSettings(Canvas source, Canvas target)
        {
            target.renderMode = source.renderMode;
            target.pixelPerfect = source.pixelPerfect;
            target.sortingOrder = source.sortingOrder;
            target.targetDisplay = source.targetDisplay;
            target.overrideSorting = source.overrideSorting;
            target.worldCamera = source.worldCamera;
            target.additionalShaderChannels = source.additionalShaderChannels;
        }

        private void CopyCanvasScalerSettings(CanvasScaler source, CanvasScaler target)
        {
            target.uiScaleMode = source.uiScaleMode;
            target.referenceResolution = source.referenceResolution;
            target.screenMatchMode = source.screenMatchMode;
            target.matchWidthOrHeight = source.matchWidthOrHeight;
            target.scaleFactor = source.scaleFactor;
            target.referencePixelsPerUnit = source.referencePixelsPerUnit;
        }

        private void CopyRaycasterSettings(GraphicRaycaster source, GraphicRaycaster target)
        {
            target.ignoreReversedGraphics = source.ignoreReversedGraphics;
            target.blockingObjects = source.blockingObjects;
            target.blockingMask = source.blockingMask;
        }

        private void DisposeAllTasks()
        {
            foreach (var poolTask in _activePoolTasks.Values)
                poolTask.Dispose();
        }

        private void OnDisable()
        {
            DisposeAllTasks();
        }
    }
}
