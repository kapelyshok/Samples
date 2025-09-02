namespace AtomicApps.Infrastructure.Bootstrap
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
}