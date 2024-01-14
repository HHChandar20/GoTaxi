using System.ComponentModel.DataAnnotations;

namespace GoTaxi.DAL.Models
{
    public class Driver : User
    {
        [Key]
        public string PlateNumber { get; set; }

        public Driver()
        {
            PlateNumber = "NotDefined";
        }

    }
}