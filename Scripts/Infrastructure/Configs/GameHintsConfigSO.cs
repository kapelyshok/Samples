using System;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicApps.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = nameof(GameHintsConfigSO), menuName = "ScriptableObjects/Configs/" + nameof(GameHintsConfigSO))]
    public class GameHintsConfigSO : ScriptableObject
    {
        public List<GameHintMapping> GameHintMappings;
    }

    [Serializable]
    public class GameHintMapping
    {
        public string GameHint;
    }
}