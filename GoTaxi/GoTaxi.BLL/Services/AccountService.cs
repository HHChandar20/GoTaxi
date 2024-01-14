using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Services
{
    public class AccountService : IAccountService
    {
        private static AccountRepository repositoryInstance = AccountRepository.GetInstance();

        public List<Account> GetAccounts()
        {
            return repositoryInstance.GetAccounts();
        }

        public Account ConvertToAccount(int id, string username, string password)
        {
            Account account = new Account();
            account.Id = id;
            account.Username = username;
            account.Password = password;

            return account;
        }

        public void AddAccount(int id, string username, string password)
        {
            repositoryInstance.AddAccount(ConvertToAccount(id, username, password));
        }

        public bool AuthenticateAccount(string username, string password)
        {
            List<Account> accounts = GetAccounts();

            if (accounts != null && accounts.Any())
            {
                foreach (Account account in accounts)
                {
                    if (account.Username == username && account.Password == password)
                    {
                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine("yes");
            }

            return false;
        }

    }
}