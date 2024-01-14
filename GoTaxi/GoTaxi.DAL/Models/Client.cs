using System.ComponentModel.DataAnnotations;

namespace GoTaxi.DAL.Models
{
    public class Client : User
    {
        [Key]
        public string PhoneNumber { get; set; }
        public int Reports { get; set; }
        public string? Destination { get; set; }
        public float DestinationLongitude { get; set; }
        public float DestinationLatitude { get; set; }
        public string? ClaimedBy { get; set; }

        public Client()
        {
            PhoneNumber = "0000000000";
            Reports = 0;
            Destination = null;
            DestinationLongitude = 0;
            DestinationLatitude = 0;
            ClaimedBy = null;
        }


    }
}