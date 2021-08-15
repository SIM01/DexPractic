using System;

namespace BankSystem.Models
{
    public class Employee : IPerson
    {
        public string Fio { get; set; }
        public int PassNom { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Position { get; set; }
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

            if (!(obj is Employee))
            {
                return false;
            }

            Employee result = (Employee) obj;
            return result.Fio == Fio && result.PassNom == PassNom;
        }
    }
}