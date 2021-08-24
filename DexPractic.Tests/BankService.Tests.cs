using System;
using System.Collections.Generic;
using BankSystem.Models;
using BankSystem.Service;
using System.Linq;
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
    }
}