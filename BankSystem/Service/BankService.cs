using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BankSystem.Exceptions;
using BankSystem.Models;
using Newtonsoft.Json;

namespace BankSystem.Service
{
    public class BankService
    {
        public delegate decimal BankServiceConvertHandler<T, U>(decimal sumin, T valin, U valout)
            where T : ICurrency where U : ICurrency;

        public List<Client> clients = new List<Client>();
        public List<Employee> employes = new List<Employee>();

        Dictionary<Client, List<Accounts>> bankklients = new Dictionary<Client, List<Accounts>>();
        Dictionary<int, List<Accounts>> newbankklients = new Dictionary<int, List<Accounts>>();

        public BankService()
        {
            DataBaseCreate(clients.GetType().GetGenericArguments().Single().Name);
            DataBaseCreate(employes.GetType().GetGenericArguments().Single().Name);
            DataBaseCreate(newbankklients.GetType().GetGenericArguments()[1].GetGenericArguments().Single().Name);

            clients = DataRequest<Client>(clients).Item1;
            employes = DataRequest<Employee>(employes).Item1;
            newbankklients = DataRequest<People>(null, newbankklients).Item2;
        }


        private Tuple<List<T>, Dictionary<int, List<Accounts>>> DataRequest<T>(List<T> person,
            Dictionary<int, List<Accounts>> dictionaryacc = null) where T : People
        {
            string path;
            if (person is null)
            {
                path = GetFilePath(dictionaryacc.GetType().GetGenericArguments()[1].GetGenericArguments().Single()
                    .Name);
            }
            else
            {
                path = GetFilePath(person.GetType().GetGenericArguments().Single().Name);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                byte[] arraytest = new byte[fileStream.Length];
                fileStream.Read(arraytest, 0, arraytest.Length);
                string readText = System.Text.Encoding.Default.GetString(arraytest);
                if (readText.Length != 0)
                {
                    if (person is null)
                    {
                        dictionaryacc = JsonConvert.DeserializeObject<Dictionary<int, List<Accounts>>>(readText);
                    }
                    else
                    {
                        person = JsonConvert.DeserializeObject<List<T>>(readText);
                    }
                }
            }

            return Tuple.Create(person, dictionaryacc);
        }

        public void OverwriteFile<T>(List<T> person, Dictionary<int, List<Accounts>> dictionaryacc = null)
            where T : People
        {
            string path;
            string textToInsert;
            if (person is null)
            {
                path = GetFilePath(dictionaryacc.GetType().GetGenericArguments()[1].GetGenericArguments().Single()
                    .Name);
                textToInsert = JsonConvert.SerializeObject(dictionaryacc);
            }
            else
            {
                path = GetFilePath(person.GetType().GetGenericArguments().Single().Name);
                textToInsert = JsonConvert.SerializeObject(person);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(textToInsert);
                fileStream.Write(array, 0, array.Length);
            }
        }

        private void DataBaseCreate(string typ)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetFilePath(null));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            if (!File.Exists(GetFilePath(typ)))
            {
                File.Create(GetFilePath(typ));
            }
        }

        private string GetFilePath(string typ)
        {
            string path;
            switch (typ)
            {
                case "Client":
                    path = Path.Combine("NOSQL_DB") + Path.DirectorySeparatorChar.ToString() + "TABLE_CLIENTS.csv";
                    break;

                case "Employee":
                    path = Path.Combine("NOSQL_DB") + Path.DirectorySeparatorChar.ToString() + "TABLE_EMPLOYES.csv";
                    break;

                case "Accounts":
                    path = Path.Combine("NOSQL_DB") + Path.DirectorySeparatorChar.ToString() + "TABLE_ACCOUNTS.csv";
                    break;

                default:
                    path = Path.Combine("NOSQL_DB");
                    break;
            }

            return path;
        }

        public Employee FindEmployee(Employee person)
        {
            return (Employee) Find(person);
        }

        public Client FindClient(Client person)
        {
            return (Client) Find(person);
        }

        private People Find<T>(T person) where T : People
        {
            if (person is Client)
            {
                return clients.Where(p => p.PassNom.Equals(person.PassNom)).SingleOrDefault();
            }

            if (person is Employee)
            {
                return (Employee) employes.Where(p => p.PassNom.Equals(person.PassNom));
            }

            return null;
        }

        public void ClientAccount(Client person, Accounts account)
        {
            if (newbankklients is null)
            {
                List<Accounts> clientacc = new List<Accounts>();
                clientacc.Add(account);
                newbankklients.Add(person.PassNom, clientacc);
            }
            else
            {
                if (newbankklients.ContainsKey(person.PassNom))
                {
                    List<Accounts> clientacc = newbankklients[person.PassNom];
                    clientacc.Add(account);
                    newbankklients[person.PassNom] = clientacc;
                }
                else
                {
                    List<Accounts> clientacc = new List<Accounts>();
                    clientacc.Add(account);
                    newbankklients.Add(person.PassNom, clientacc);
                }
            }

            OverwriteFile<People>(null, newbankklients);
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
                if (clients is not null)
                {
                    if (!clients.Contains((Client) (IPerson) person))
                    {
                        clients.Add((Client) (IPerson) person);
                        OverwriteFile<Client>(clients);
                    }
                }
                else
                {
                    clients.Add((Client) (IPerson) person);
                    OverwriteFile<Client>(clients);
                }
            }

            if (person is Employee)
            {
                if (!employes.Contains((Employee) (IPerson) person))
                {
                    employes.Add((Employee) (IPerson) person);
                    OverwriteFile<Employee>(employes);
                }
            }
        }
    }
}