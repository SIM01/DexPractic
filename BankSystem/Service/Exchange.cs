using System;
using BankSystem.Models;

namespace BankSystem.BankService
{
    public class Exchange : IExchange
    {
        public decimal CurrencyConverter<T, U>(decimal sumin, T valin, U valout) where T : ICurrency where U : ICurrency
        {
            return Convert.ToDecimal(
                (Convert.ToDouble(sumin) * valin.CurrencyRate) / valout.CurrencyRate
            );
        }
    }
}