using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AtomicApps
{
    public class SceneContextReadySignal
    {
        public SceneContext SceneContext;

        public SceneContextReadySignal(SceneContext sceneContext)
        {
            SceneContext = sceneContext;
        }
    }
}
