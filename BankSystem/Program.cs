using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankSystem.Service;
using BankSystem.Models;
using Bogus;

namespace BankSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var klientaccountDebet = new Accounts()
            {
                Account = new //Eur() 
                    DefaultCurrency() {CurrencyRate = 1.2},
                Sum = 100
            };

            var klientaccountKredit = new Accounts()
            {
                Account = new //Usd() 
                    DefaultCurrency() {CurrencyRate = 1},
                Sum = 200
            };
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
/*
            foreach (var men in bank.clients)
            {
                Console.WriteLine(
                    $"ФИО:{men.Fio}, № паспорта:{men.PassNom}, дата рождения:{men.DateOfBirth.ToString("dd/mm/yyyy")};");
            }
*/
            var clientGenerator = new Faker<Client>("ru")
                    .RuleFor(x => x.Fio, f => f.Name.FullName())
                    .RuleFor(x => x.PassNom, f => f.Random.Int(11111, 99999))
                    .RuleFor(x => x.DateOfBirth, f => f.Date.Past(60, DateTime.Now.AddYears(-18)))
                ;

            var fakeClients = clientGenerator.Generate(10).ToList();


            var employeeGenerator = new Faker<Employee>("ru")
                .RuleFor(x => x.Fio, f => f.Name.FullName())
                .RuleFor(x => x.PassNom, f => f.Random.Int(111111, 999999))
                .RuleFor(x => x.DateOfBirth, f => f.Date.Past(60, DateTime.Now.AddYears(-18)))
                .RuleFor(x => x.Position, f => f.Random.ListItem(new List<string>
                {
                    "Программист",
                    "Доктор",
                    "Маляр",
                    "Дизайнер",
                    "Слесарь",
                    "Милиционер",
                    "Дворник",
                    "Менеджер"
                }));
            var fakeEmployer = employeeGenerator.Generate(10).ToList();

            var accountGenerator = new Faker<Accounts>("ru")
                    .RuleFor(x => x.Account, f => new DefaultCurrency() {CurrencyRate = 1.2, CurrencyName = "UAH"})
                    .RuleFor(x => x.Sum, f => f.Random.Decimal(0, 100000))
                ;

            var fakeAccounts = accountGenerator.Generate(10).ToList();


            var exp = new ExportData();
            for (int i = 0; i < 10; i++)
            {
                string path = Path.Combine("NOSQL_DB") + Path.DirectorySeparatorChar.ToString() + "ACC_" + i + ".txt";
                exp.ExportToFile(path, fakeAccounts[i]);
            }

            var sumconv = await exchange.CurrencyConverterOnline(200, new Usd(), new Eur());

            for (int i = 0; i < 10; i++)
            {
                var countdown = new CountdownEvent(2);
                var locker = new object();
                ThreadPool.QueueUserWorkItem
                (
                    new WaitCallback
                    (
                        (_) =>
                        {
                            lock (locker) // синхронизация
                            {
                                bank.Add<Client>(fakeClients[i]);
                            }

                            countdown.Signal();
                        }
                    )
                );
                ThreadPool.QueueUserWorkItem
                (
                    new WaitCallback
                    (
                        (_) =>
                        {
                            lock (locker) // синхронизация
                            {
                                ShowClient(bank.clients);
                            }

                            countdown.Signal();
                        }
                    )
                );

                countdown.Wait();

                Console.WriteLine("-----------------------------------");
                //bank.Add<Employee>(fakeEmployer[i]);
                //bank.ClientAccount(fakeClients[i], fakeAccounts[i]);
            }
        }

        public static void ShowClient(List<Client> myClients)
        {
            foreach (var men in myClients)
            {
                Console.WriteLine(
                    $"ФИО:{men.Fio}, № паспорта:{men.PassNom}, дата рождения:{men.DateOfBirth.ToString("dd/mm/yyyy")};");
            }
        }
    }
}