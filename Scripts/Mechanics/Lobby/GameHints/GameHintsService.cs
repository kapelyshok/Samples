using System.Collections.Generic;
using System.Linq;
using AtomicApps.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Lobby.GameHints
{
    public class GameHintsService : MonoBehaviour, IGameHintsService
    {
        private GameHintsConfigSO _config;
        private GameHintMapping _lastMapping;

        private readonly List<GameHintMapping> _candidates = new List<GameHintMapping>(8);

        [Inject]
        private void Construct(GameConfigSO config)
        {
            _config = config.GameHintsConfig;
            _lastMapping = null;
        }

        public GameHintMapping GetRandomHintMapping()
        {
            var list = _config?.GameHintMappings;
            if (list == null || list.Count == 0) return null;
            if (list.Count == 1)
            {
                _lastMapping = list[0];
                return _lastMapping;
            }

            _candidates.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var m = list[i];
                if (!ReferenceEquals(m, _lastMapping))
                    _candidates.Add(m);
            }

            if (_candidates.Count == 0)
            {
                int any = Random.Range(0, list.Count);
                _lastMapping = list[any];
                return _lastMapping;
            }

            int pick = Random.Range(0, _candidates.Count);
            _lastMapping = _candidates[pick];
            return _lastMapping;
        }
    }
}
