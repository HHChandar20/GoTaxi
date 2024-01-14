using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoTaxi.DAL.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        public int Reports { get; set; }
        public int? UserId { get; set; }
        public int? DestinationId { get; set; }
        [MaxLength(15)]
        public int? DriverId { get; set; }

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