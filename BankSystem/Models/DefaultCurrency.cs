namespace BankSystem.Models
{
    public class DefaultCurrency:ICurrency
    {
        public double CurrencyRate { get; set; }
        public string CurrencyName { get; set; }
    }
}