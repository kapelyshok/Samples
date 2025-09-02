using System;
using System.Collections.Generic;
using AtomicApps.Infrastructure.Currencies;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers.Boosters;
using UnityEngine;

namespace AtomicApps.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = nameof(BoosterPricesConfigSO), menuName = "ScriptableObjects/Configs/" + nameof(BoosterPricesConfigSO))]
    public class BoosterPricesConfigSO : ScriptableObject
    {
        public List<BoosterPriceData> BoosterPrices;
    }

    [Serializable]
    public class BoosterPriceData
    {
        public BoosterType BoosterType;
        public CurrencyType CurrencyType;
        public int Cost;
        public int Amount;
    }
}