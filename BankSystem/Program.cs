using System;
using BankSystem.Service;
using BankSystem.Models;

namespace BankSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var klientaccountDebet = new Accounts()
                {Account = new Eur() {CurrencyRate = 1.2}, Sum = 100};

            var klientaccountKredit = new Accounts()
                {Account = new Usd() {CurrencyRate = 1}, Sum = 200};
            var bank = new BankService();
            var exchange = new Exchange();
            decimal sumin = 50;
            var valin = klientaccountDebet.Account;
            var valout = klientaccountKredit.Account;
            
            var convHandler = new BankService.BankServiceConvertHandler<ICurrency, ICurrency>(exchange.CurrencyConverter);
           
            
            bank.MoneyTransfer(sumin,klientaccountDebet,klientaccountKredit, convHandler);
            
            Func<decimal, ICurrency, ICurrency, decimal> convertF = exchange.CurrencyConverter;
            bank.MoneyTransferFunc(sumin,klientaccountDebet,klientaccountKredit,convertF);
        }
        
        
        
    }
}