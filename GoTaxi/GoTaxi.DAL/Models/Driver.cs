using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoTaxi.DAL.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        [Required]
        [MaxLength(15)]
        public string PlateNumber { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public Driver()
        {
            PlateNumber = "";
            User = new User();
        }

        public Driver(string plateNumber, User user)
        {
            PlateNumber = plateNumber;
            User = user;
        }
    }
}