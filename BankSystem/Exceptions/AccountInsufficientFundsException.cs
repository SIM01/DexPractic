using System;

namespace BankSystem.Exceptions
{
    public class AccountInsufficientFundsException : Exception
    {
        public AccountInsufficientFundsException(string message) : base(message)
        {
        }
    }
}