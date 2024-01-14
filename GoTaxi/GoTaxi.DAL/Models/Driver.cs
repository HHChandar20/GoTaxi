namespace GoTaxi.DAL.Models
{
    public class Driver
    {
        public string PlateNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public Driver()
        {
            PlateNumber = "NotDefined";
            FullName = "NotDefined";
            Email = "NotDefined";
            Password = "NotDefined";
        }

    }
}