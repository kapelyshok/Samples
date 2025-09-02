namespace AtomicApps.Infrastructure.Currencies
{
    public interface ICurrenciesService
    {
        public CurrencyWallet GetCurrencyWallet(CurrencyType currencyType);
    }
}