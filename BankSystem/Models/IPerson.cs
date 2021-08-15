using System;

namespace BankSystem.Models
{
    public interface IPerson
    {
        public string Fio { get; set; }
        public int PassNom { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}