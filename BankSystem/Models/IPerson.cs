using System;

namespace BankSystem.Models
{
    public interface IPerson
    {
        public string Fio { get; set; }
        public string PassNom { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}