using System;

namespace AtomicApps.Scpts.Mechanics.Lobby.Hearts
{
    public interface IHearthService
    {
        int Count { get; }
        int Max { get; }
        bool IsMax { get; }
        bool IsFree { get; }
        bool HasHeartsToPlay { get; }
        float FreeTime { get; }

        event Action OnUpdated;

        void SpendHearth();
        bool TryAddHearth();
        void AddMax();
        void SetFreeFor(int time);
    }
}