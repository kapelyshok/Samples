using System;

namespace AtomicApps.Infrastructure.Currencies
{
    [Serializable]
    public class CurrencyValueData
    {
        public CurrencyType Currency;
        public int Value;

        public CurrencyValueData()
        {
            
        }
        
        public CurrencyValueData(CurrencyValueData data)
        {
            Currency = data.Currency;
            Value = data.Value;
        }
    }
}