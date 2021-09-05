using System;
using System.Collections.Generic;
using BankSystem.Models;
using BankSystem.Service;
using System.Linq;
using System.Threading;
using Xunit;

namespace DexPractic.Tests
{
    public class BankService_Tests
    {
        [Fact]
        public void Client_Add_Find()
        {
            //Arrange
            BankService bank = new BankService();
            bank.clients = new List<Client>();
            var testClient = new Client()
                {Fio = "Тест Тест Тест", PassNom = 123456, DateOfBirth = new DateTime(1983, 7, 20)};

            //Act
            bank.Add<Client>(testClient);
            var result = bank.FindClient(testClient);

            //Assert
            Assert.Equal(testClient, result);
        }

        [Fact]
        public void MoneyTransfer_Lock()
        {
            //Arrange
            BankService bank = new BankService();
            var klientaccountDebet1 = new Accounts()
            {
                Account = new DefaultCurrency() {CurrencyRate = 1.2},
                Sum = 100
            };

            var klientaccountDebet2 = new Accounts()
            {
                Account = new DefaultCurrency() {CurrencyRate = 1.3},
                Sum = 150
            };

            var klientaccountKredit = new Accounts()
            {
                Account = new DefaultCurrency() {CurrencyRate = 1},
                Sum = 200
            };

            var exchange = new Exchange();
            decimal sumin = 50;

            Func<decimal, ICurrency, ICurrency, decimal> convertF = exchange.CurrencyConverter;
            var locker = new object();
            var completed = 0;

            //Act
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (locker)
                {
                    bank.MoneyTransferFunc(sumin, klientaccountDebet1, klientaccountKredit, convertF);
                }

                Interlocked.Increment(ref completed);
            });
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (locker)
                {
                    bank.MoneyTransferFunc(sumin, klientaccountDebet2, klientaccountKredit, convertF);
                }

                Interlocked.Increment(ref completed);
            });
            
            while (completed < 2)
            {
                Thread.Sleep(25);
            }

            var result = klientaccountKredit.Sum;

            //Assert
            Assert.Equal(325, result);
        }
    }
}