using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Models
{
    public class Client : User
    {
        [Key]
        public string PhoneNumber { get; set; }
        public int Reports { get; set; }

        public Client()
        {
            PhoneNumber = "0000000000";
            Reports = 0;
        }


    }
}