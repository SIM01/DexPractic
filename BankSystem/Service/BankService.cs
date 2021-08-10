using System;
using System.Collections.Generic;
using System.Linq;
using BankSystem.Exceptions;
using BankSystem.Models;

namespace BankSystem.Service
{
    public class BankService
    {
        public delegate decimal BankServiceConvertHandler<T, U>(decimal sumin, T valin, U valout)
            where T : ICurrency where U : ICurrency;

        List<Client> klients = new List<Client>();
        List<Employee> employes = new List<Employee>();
        Dictionary<Client, List<Accounts>> bankklients = new Dictionary<Client, List<Accounts>>();

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

        public void ClientAccount(Client person, Accounts account)
        {
            if (bankklients.ContainsKey(person))
            {
                List<Accounts> clientacc = bankklients[person];
                clientacc.Add(account);
                bankklients[person] = clientacc;
            }
            else
            {
                List<Accounts> clientacc = new List<Accounts>();
                clientacc.Add(account);
                bankklients.Add(person, clientacc);
            }
        }

        public void MoneyTransfer(decimal sum, Accounts accountDebet, Accounts accountKredit,
            BankServiceConvertHandler<ICurrency, ICurrency> convertHandler)
        {
            if (accountDebet.Sum >= sum)
            {
                accountDebet.Sum = accountDebet.Sum - sum;
                accountKredit.Sum = accountKredit.Sum +
                                    convertHandler(sum, accountDebet.Account, accountKredit.Account);
            }
            else
            {
                throw new AccountInsufficientFundsException("Недостаточно средств на счёте!");
            }
        }

        public void MoneyTransferFunc(decimal sum, Accounts accountDebet, Accounts accountKredit,
            Func<decimal, ICurrency, ICurrency, decimal> convertF)
        {
            if (sum < 0)
            {
                throw new MoneyTransferNegativeAmountException("Невозможен перевод отрицательной суммы!");
            }

            if (accountDebet.Sum >= sum)
            {
                accountDebet.Sum = accountDebet.Sum - sum;
                accountKredit.Sum = accountKredit.Sum + convertF(sum, accountDebet.Account, accountKredit.Account);
            }
            else
            {
                throw new AccountInsufficientFundsException("Недостаточно средств на счёте!");
            }
        }

        public void Add<T>(T person) where T : IPerson
        {
            if (((DateTime.Today).Year - person.DateOfBirth.Year) < 18)
            {
                throw new ClientAgeLimitException("Данный клиент/сотрудник несовершеннолетний!");
            }

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