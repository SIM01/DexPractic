namespace BankSystem.Models
{
    public class Employee : IPerson
    {
        public string Fio { get; set; }

        public string PassNom { get; set; }

        public string Position { get; set; }
    }
}