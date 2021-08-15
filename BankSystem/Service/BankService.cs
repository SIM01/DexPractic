using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BankSystem.Exceptions;
using BankSystem.Models;

namespace BankSystem.Service
{
    public class BankService
    {
        public delegate decimal BankServiceConvertHandler<T, U>(decimal sumin, T valin, U valout)
            where T : ICurrency where U : ICurrency;

        public List<Client> clients = new List<Client>();
        public List<Employee> employes = new List<Employee>();

        Dictionary<Client, List<Accounts>> bankklients = new Dictionary<Client, List<Accounts>>();

        public BankService()
        {
            DataBaseCreate<Client>(new Client());

            DataBaseCreate<Client>(new Client(), 1);

            DataBaseCreate<Employee>(new Employee());

            DataRequest(GetFilePath<IPerson>(new Client()));
            DataRequest(GetFilePath<IPerson>(new Employee()));

            DataRequest(GetFilePath<IPerson>(new Client(), 1));
        }

        private void DataBaseCreate<T>(T person, int optionalint = 0) where T : IPerson
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetFilePath<IPerson>(null));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            if (!File.Exists(GetFilePath<IPerson>(person, optionalint)))
            {
                File.Create(GetFilePath<IPerson>(person, optionalint));
            }
        }

        private string GetFilePath<T>(T person, int optionalint = 0) where T : IPerson
        {
            string path = Path.Combine("NOSQL_DB");

            if (person is Client)
            {
                if (optionalint == 0)
                {
                    path = path + Path.DirectorySeparatorChar.ToString() + "TABLE_CLIENTS.csv";
                }
                else
                {
                    path = path + Path.DirectorySeparatorChar.ToString() + "TABLE_ACCOUNTS.csv";
                }
            }


            if (person is Employee)
            {
                path = path + Path.DirectorySeparatorChar.ToString() + "TABLE_EMPLOYES.csv";
            }

            return path;
        }

        private void DataRequest(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                byte[] arraytest = new byte[fileStream.Length];
                fileStream.Read(arraytest, 0, arraytest.Length);
                string readText = System.Text.Encoding.Default.GetString(arraytest);

                string[] clientStrings = readText.Split('#');
                foreach (string kli in clientStrings)
                {
                    if (kli.Split('|').Length > 1)
                    {
                        string[] clientAcc = kli.Split('|');
                        int count = 0;
                        var client = new Client();
                        List<Accounts> accounts = new List<Accounts>();
                        foreach (string kliAcc in clientAcc)
                        {
                            if (count == 0)
                            {
                                string[] clientFields = kliAcc.Split(';');
                                if (kliAcc.Split(';').Length == 3)
                                {
                                    client.Fio = clientFields[0];
                                    client.PassNom = Convert.ToInt32(clientFields[1]);
                                    client.DateOfBirth = Convert.ToDateTime(clientFields[2]);
                                }
                            }
                            else
                            {
                                var account = new Accounts();
                                string[] accFields = kliAcc.Split(';');

                                account.Account = new DefaultCurrency
                                {
                                    CurrencyRate = Math.Round(Convert.ToDouble(accFields[0]), 4),
                                    CurrencyName = accFields[1]
                                };
                                account.Sum = Convert.ToDecimal(accFields[2]);

                                accounts.Add(account);
                            }

                            count++;
                        }

                        bankklients.Add(client, accounts);
                    }
                    else
                    {
                        string[] clientFields = kli.Split(';');
                        if (kli.Split(';').Length == 3)
                        {
                            var client = new Client()
                            {
                                Fio = clientFields[0],
                                PassNom = Convert.ToInt32(clientFields[1]),
                                DateOfBirth = Convert.ToDateTime(clientFields[2])
                            };
                            clients.Add(client);
                        }

                        if (kli.Split(';').Length == 4)
                        {
                            var client = new Employee()
                            {
                                Fio = clientFields[0],
                                PassNom = Convert.ToInt32(clientFields[1]),
                                DateOfBirth = Convert.ToDateTime(clientFields[2]),
                                Position = clientFields[3]
                            };
                            employes.Add(client);
                        }
                    }
                }
            }
        }

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
                return (IPerson) clients.Where(p => p.PassNom.Equals(person.PassNom));
            }

            if (person is Employee)
            {
                return (IPerson) employes.Where(p => p.PassNom.Equals(person.PassNom));
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

            OverwriteFile(GetFilePath<IPerson>(new Client(), 1));
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
                if (!clients.Contains((Client) (IPerson) person))
                {
                    clients.Add((Client) (IPerson) person);
                    AddToFile<IPerson>(GetFilePath<IPerson>(person), person);
                }
            }

            if (person is Employee)
            {
                if (!employes.Contains((Employee) (IPerson) person))
                {
                    employes.Add((Employee) (IPerson) person);
                    AddToFile<IPerson>(GetFilePath<IPerson>(person), person);
                }
            }
        }

        public void AddToFile<T>(string path, T person) where T : IPerson
        {
            PropertyInfo[] myProperty = person.GetType().GetProperties();
            string textToInsert = "#";
            foreach (PropertyInfo f in myProperty)
            {
                textToInsert = textToInsert + f.GetValue(person) + ";";
            }

            textToInsert = textToInsert.Substring(0, textToInsert.Length - 1);
            using (FileStream fileStream = new FileStream(path, FileMode.Append))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(textToInsert);
                fileStream.Write(array, 0, array.Length);
            }
        }

        public void OverwriteFile(string path)
        {
            string textToInsert = "";
            foreach (var kli in bankklients.Keys)
            {
                PropertyInfo[] myProperty = kli.GetType().GetProperties();
                textToInsert = textToInsert + "#";
                foreach (PropertyInfo f in myProperty)
                {
                    textToInsert = textToInsert + f.GetValue(kli) + ";";
                }

                textToInsert = textToInsert.Substring(0, textToInsert.Length - 1);
                List<Accounts> clientacc = new List<Accounts>();
                clientacc = bankklients[kli];
                foreach (var acc in clientacc)
                {
                    textToInsert = textToInsert + "|";

                    textToInsert = textToInsert + acc.Account.CurrencyRate.ToString() + ";";
                    textToInsert = textToInsert + acc.Account.CurrencyName + ";";
                    textToInsert = textToInsert + acc.Sum.ToString() + ";";
                }
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(textToInsert);
                fileStream.Write(array, 0, array.Length);
            }
        }
    }
}