using System;
using System.Collections.Generic;
using BankSystem.Service;
using BankSystem.Models;

namespace BankSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var klientaccountDebet = new Accounts()
                {Account = new //Eur() 
                    DefaultCurrency() {CurrencyRate = 1.2}, Sum = 100};

            var klientaccountKredit = new Accounts()
                {Account = new //Usd() 
                    DefaultCurrency(){CurrencyRate = 1}, Sum = 200};
            var bank = new BankService();
            var exchange = new Exchange();
            decimal sumin = 50;
            var valin = klientaccountDebet.Account;
            var valout = klientaccountKredit.Account;

            var convHandler =
                new BankService.BankServiceConvertHandler<ICurrency, ICurrency>(exchange.CurrencyConverter);


            bank.MoneyTransfer(sumin, klientaccountDebet, klientaccountKredit, convHandler);

            Func<decimal, ICurrency, ICurrency, decimal> convertF = exchange.CurrencyConverter;
            bank.MoneyTransferFunc(sumin, klientaccountDebet, klientaccountKredit, convertF);

            foreach (var men in bank.clients)
            {
                Console.WriteLine(
                    $"ФИО:{men.Fio}, № паспорта:{men.PassNom}, дата рождения:{men.DateOfBirth.ToString("dd/mm/yyyy")};");
            }

            bank.Add<Client>(new Client()
                {Fio = "Тест Тест Тест", PassNom = 123456, DateOfBirth = new DateTime(1983, 7, 20)});

            var klientaccount1 = new Accounts()
                {Account = new DefaultCurrency() {CurrencyRate = 1.2, CurrencyName = "UAH"}, Sum = 100};

            var klientaccount2 = new Accounts()
                {Account = new DefaultCurrency() {CurrencyRate = 1, CurrencyName = "RUB"}, Sum = 200};

            var newCli = new Client()
                {Fio = "Тест2 Тест2 Тест2", PassNom = 654123, DateOfBirth = new DateTime(1988, 6, 20)};
            bank.ClientAccount(newCli, klientaccount1);
            bank.ClientAccount(newCli, klientaccount2);
        }
    }
}