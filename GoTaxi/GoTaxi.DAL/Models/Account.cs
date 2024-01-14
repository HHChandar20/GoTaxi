using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        public Account()
        {
            Id = 0;
            Username = "NotDefined";
            Password = "NotDefined";
        }

    }
}