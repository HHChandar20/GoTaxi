using System.ComponentModel.DataAnnotations;

namespace GoTaxi.DAL.Models
{
    public class Driver
    {
        [Key]
        public string PlateNumber { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }


        public Driver()
        {
            PlateNumber = "NotDefined";
            Email = "NotDefined";
            FullName = "NotDefined";
            Password = "NotDefined";
            Longitude = 0.1;
            Latitude = 0.1;
        }

    }
}