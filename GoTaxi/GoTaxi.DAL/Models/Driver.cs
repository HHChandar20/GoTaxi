namespace GoTaxi.DAL.Models
{
    public class Driver
    {
        public string PlateNumber { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public Driver()
        {
            PlateNumber = "NotDefined";
            Username = "NotDefined";
            Email = "NotDefined";
            Password = "NotDefined";
        }

    }
}