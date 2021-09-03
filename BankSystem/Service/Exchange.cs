using System;
using System.Threading.Tasks;
using BankSystem.Models;

namespace BankSystem.Service
{
    public class Exchange : IExchange
    {
        public decimal CurrencyConverter<T, U>(decimal sumin, T valin, U valout) where T : ICurrency where U : ICurrency
        {
            return Convert.ToDecimal(
                (Convert.ToDouble(sumin) * valin.CurrencyRate) / valout.CurrencyRate
            );
        }
        
        public async Task<decimal> CurrencyConverterOnline<T, U>(decimal sumin, T valin, U valout) where T : ICurrency where U : ICurrency
        {
            var rateservice = new RateService();
            var rate = await rateservice.GetRate();
            var valuta = rate.Rates;
            var tmpProperty = valuta.GetType().GetProperties();
            double ratein = 1;
            double rateout = 1;
            foreach (var p in tmpProperty)
            {
                if (p.Name == valin.GetType().Name.ToUpper())
                {
                    ratein = (double)p.GetValue(valuta);  
                }
                if (p.Name == valout.GetType().Name.ToUpper())
                {
                    rateout = (double)p.GetValue(valuta);  
                }
            }
            return Convert.ToDecimal((Convert.ToDouble(sumin) / ratein) * rateout);
        }
    }
}