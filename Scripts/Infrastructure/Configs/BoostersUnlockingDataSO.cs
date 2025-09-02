using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers.Boosters;
using UnityEngine;

namespace AtomicApps.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = nameof(BoostersUnlockingDataSO), menuName = "ScriptableObjects/Configs/" + nameof(BoostersUnlockingDataSO))]
    public class BoostersUnlockingDataSO : ScriptableObject
    {
        public List<BoosterUnlockingData> BoosterUnlockingDatas;

        public BoosterUnlockingData GetCurrentLevelData(int levelIndex)
        {
            return BoosterUnlockingDatas.FirstOrDefault(data => data.LevelToOpen == levelIndex);
        }
    }

    [Serializable]
    public class BoosterUnlockingData
    {
        public BoosterType BoosterType;
        public int LevelToOpen;
    }
}