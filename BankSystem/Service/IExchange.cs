using BankSystem.Models;

namespace BankSystem.Service
{
    public interface IExchange
    {
        public decimal CurrencyConverter<T, U>(decimal sumin, T valin, U valout)
            where T : ICurrency where U : ICurrency;
    }
}