using BankSystem.Models;

namespace BankSystem.BankService
{
    public interface IExchange
    {
        public decimal CurrencyConverter<T, U>(decimal sumin, T valin, U valout)
            where T : ICurrency where U : ICurrency;
    }
}