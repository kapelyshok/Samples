using AtomicApps.Infrastructure.Configs;

namespace AtomicApps.Mechanics.Lobby.GameHints
{
    public interface IGameHintsService
    {
        public GameHintMapping GetRandomHintMapping();
    }
}