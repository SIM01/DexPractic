using System;

namespace BankSystem.Exceptions
{
    public class ClientAgeLimitException: Exception
    {
        public ClientAgeLimitException(string message) : base(message)
        {
        }
    }
}