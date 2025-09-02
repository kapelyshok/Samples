using Zenject;

namespace AtomicApps
{
    public class SceneContextSignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var sceneContext = GetComponent<SceneContext>();
            sceneContext.OnPostResolve.AddListener(() =>
            {
                var signalBus = Container.Resolve<SignalBus>();
                signalBus.Fire(new SceneContextReadySignal(GetComponent<SceneContext>()));
            });
        }
    }
}