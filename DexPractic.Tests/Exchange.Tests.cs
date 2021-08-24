using System;
using BankSystem.Models;
using BankSystem.Service;
using Xunit;

namespace DexPractic.Tests
{
    public class Exchange_Tests
    {
        [Fact]
        public void CurrencyConverter_100_USD_To_EUR()
        {
            //Arrange
            Exchange exchange = new Exchange();
            var valin = new Usd() {CurrencyRate = 1};
            var valout = new Eur() {CurrencyRate = 1.2};
            decimal sum = 10000;
            decimal rightresult = Convert.ToDecimal((Convert.ToDouble(sum) * valin.CurrencyRate) / valout.CurrencyRate);
            //Act
            var result = exchange.CurrencyConverter(sum, valin, valout);
            //Assert
            Assert.Equal(rightresult, result);
        }

        [Fact]
        public void CurrencyConverter_DivideByZero()
        {
            //Arrange
            Exchange exchange = new Exchange();
            var valin = new Usd() {CurrencyRate = 1};
            var valout = new Eur() {CurrencyRate = 0};
            decimal sum = 10000;
            //Assert.Throws<DivideByZeroException>(() =>
            Assert.Throws<OverflowException>(() =>
            {
                //Act
                var result = exchange.CurrencyConverter(sum, valin, valout);
                //Assert
                Assert.IsType<decimal>(result);
            });
        }
    }
}