using System;

namespace BankSystem.Exceptions
{
    public class MoneyTransferNegativeAmountException: Exception
    {
        public MoneyTransferNegativeAmountException(string message) : base(message)
        {
        }
    }
}