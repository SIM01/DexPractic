using System.Collections.Generic;
using System.Linq;
using BankSystem.Models;

namespace BankSystem.BankService
{
    public class BankService
    {
        List<Client> klients = new List<Client>();
        List<Employee> employes = new List<Employee>();

        public Employee FindEmployee(Employee person)
        {
            return (Employee) Find(person);
        }

        public Client FindClient(Client person)
        {
            return (Client) Find(person);
        }

        private IPerson Find<T>(T person) where T : IPerson
        {
            if (person is Client)
            {
                return (IPerson) klients.Where(p => p.PassNom.Contains(person.PassNom));
            }

            if (person is Employee)
            {
                return (IPerson) employes.Where(p => p.PassNom.Contains(person.PassNom));
            }

            return null;
        }

        public void Add<T>(T person) where T : IPerson
        {
            if (person is Client)
            {
                if (!klients.Contains((Client) (IPerson) person))
                {
                    klients.Add((Client) (IPerson) person);
                }
            }

            if (person is Employee)
            {
                if (!employes.Contains((Employee) (IPerson) person))
                {
                    employes.Add((Employee) (IPerson) person);
                }
            }
        }
    }
}