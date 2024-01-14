using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Repositories
{
    public class AccountRepository
    {
        private static AccountRepository instance = null;
        private List<Account> accountsList = new List<Account> { new Account() };

        public static AccountRepository GetInstance()
        {
            if (instance == null)
            {
                instance = new AccountRepository();
            }

            return instance;
        }

        private AccountRepository()
        {

        }

        // Read multiple
        public List<Account> GetAccounts()
        {
            return accountsList;
        }

        // Read single
        public Account GetAccountById(int id)
        {
            return accountsList.Find(account => account.Id == id);
        }

        // Create
        public void AddAccount(Account newAccount)
        {
            accountsList.Add(newAccount);
        }

    }
}