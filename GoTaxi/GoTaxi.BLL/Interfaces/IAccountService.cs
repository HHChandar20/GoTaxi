using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Interfaces
{
    public interface IAccountService
    {
        void AddAccount(int id, string username, string password);
        bool AuthenticateAccount(string username, string password);
        Account ConvertToAccount(int id, string username, string password);
        List<Account> GetAccounts();
    }
}