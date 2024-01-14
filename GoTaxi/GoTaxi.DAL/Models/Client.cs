using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoTaxi.DAL.Models
{
    public class Client
    {
        private int clientId;
        private string phoneNumber;
        private int reports;
        private int? userId;
        private int? destinationId;
        private int? driverId;

        [Key]
        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        public int Reports
        {
            get { return reports; }
            set { reports = value; }
        }

        public int? UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public int? DestinationId
        {
            get { return destinationId; }
            set { destinationId = value; }
        }

        [MaxLength(15)]
        public int? DriverId
        {
            get { return driverId; }
            set { driverId = value; }
        }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("DestinationId")]
        public Destination? Destination { get; set; }

        [ForeignKey("DriverId")]
        public Driver? ClaimedBy { get; set; }


        public Client()
        {
            PhoneNumber = "";
            Reports = 0;
            ClaimedBy = null;
            User = null;
            Destination = null;
        }

        public Client(string phoneNumber, User user)
        {
            PhoneNumber = phoneNumber;
            Reports = 0;
            ClaimedBy = null;
            User = user;
            Destination = new Destination();
        }
    }
}