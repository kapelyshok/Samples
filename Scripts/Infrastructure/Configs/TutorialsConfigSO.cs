using System;
using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.Tutorials;
using UnityEngine;

namespace AtomicApps.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = nameof(TutorialsConfigSO), menuName = "ScriptableObjects/Configs/" + nameof(TutorialsConfigSO))]
    public class TutorialsConfigSO : ScriptableObject
    {
        public List<TutorialGroupData> TutorialGroups;
    }
}