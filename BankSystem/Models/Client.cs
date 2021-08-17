using System;

namespace BankSystem.Models
{
    public class Client : People
    {
        public override int GetHashCode()
        {
            return PassNom;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Client))
            {
                return false;
            }

            Client result = (Client) obj;
            return result.Fio == Fio && result.PassNom == PassNom;
        }
    }
}