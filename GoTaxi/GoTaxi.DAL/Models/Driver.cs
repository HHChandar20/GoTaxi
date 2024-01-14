using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoTaxi.DAL.Models
{
    public class Driver
    {
        private int driverId;
        private string plateNumber;
        private int? userId;
        private User? user;

        [Key]
        public int DriverId
        {
            get { return driverId; }
            set { driverId = value; }
        }

        [Required]
        [MaxLength(15)]
        public string PlateNumber
        {
            get { return plateNumber; }
            set { plateNumber = value; }
        }

        public int? UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [ForeignKey("UserId")]
        public User? User
        {
            get { return user; }
            set { user = value; }
        }

        public Driver()
        {
            plateNumber = "";
            user = null;
        }

        public Driver(string plateNumber, User user)
        {
            this.plateNumber = plateNumber;
            this.user = user;
        }
    }
}