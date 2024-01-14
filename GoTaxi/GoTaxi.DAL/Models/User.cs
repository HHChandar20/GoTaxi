using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Models
{
    public class User
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        protected User()
        {
            Email = "NotDefined";
            FullName = "NotDefined";
            Password = "NotDefined";
            Longitude = 0.0;
            Latitude = 0.0;
        }
    }
}