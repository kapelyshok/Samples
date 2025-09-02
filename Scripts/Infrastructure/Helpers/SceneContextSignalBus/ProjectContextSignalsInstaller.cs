using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AtomicApps
{
    public class ProjectContextSignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<SceneContextReadySignal>();
        }
    }
}
