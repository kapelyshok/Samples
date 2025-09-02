using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;

namespace AtomicApps.Infrastructure.Currencies
{
    [Serializable]
    public class CurrenciesSaveData : SavableData
    {
        public List<CurrencyValueData> CurrenciesData = new List<CurrencyValueData>();

        public CurrencyValueData GetCurrencyValue(CurrencyType currencyType)
        {
            return CurrenciesData.Find(x => x.Currency == currencyType);
        }
        
        public override void SetDefaultValues(object inData = null)
        {
            CurrenciesData.Clear();
            
            var defaultValues = inData == null ? new CurrenciesSaveData() : inData as CurrenciesSaveData;

            if (defaultValues != null)
            {
                CurrenciesData = defaultValues.CurrenciesData;
            }
            else
            {
                foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
                {
                    CurrenciesData.Add(new CurrencyValueData(){Currency = currency, Value = 0});
                }
            }
        }
    }
}