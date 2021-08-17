using System;

namespace BankSystem.Models
{
    public class People : IPerson
    {
        public string Fio { get; set; }
        public int PassNom { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}