using System.Collections.Generic;
using UnityEngine;

namespace AtomicApps.Infrastructure.Currencies
{
    [CreateAssetMenu(fileName = nameof(CurrenciesConfigSO), menuName = "ScriptableObjects/Configs/" + nameof(CurrenciesConfigSO))]
    public class CurrenciesConfigSO : ScriptableObject
    {
        public List<CurrencyValueData> CurrenciesData = new List<CurrencyValueData>();
    }
}