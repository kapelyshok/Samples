using System;

namespace AtomicApps.Mechanics.Lobby.Hearts
{
    public interface IHeartRegenService
    {
        event Action<int> OnIncreameantHearts;
        string LeftTimeToString { get; }
        void Resume();
        void Stop();
        void ResetTimer();
        void Initialize();
    }
}