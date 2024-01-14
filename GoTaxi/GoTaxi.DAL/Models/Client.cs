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
        private User? user;
        private Destination? destination;
        private Driver? claimedBy;


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
        public User? User
        {
            get { return user; }
            set { user = value; }
        }

        [ForeignKey("DestinationId")]
        public Destination? Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        [ForeignKey("DriverId")]
        public Driver? ClaimedBy
        {
            get { return claimedBy; }
            set { claimedBy = value; }
        }


        public Client()
        {
            phoneNumber = "";
            reports = 0;
            claimedBy = null;
            user = null;
            destination = null;
        }

        public Client(string phoneNumber, User user)
        {
            this.phoneNumber = phoneNumber;
            reports = 0;
            claimedBy = null;
            this.user = user;
            destination = new Destination();
        }
    }
}