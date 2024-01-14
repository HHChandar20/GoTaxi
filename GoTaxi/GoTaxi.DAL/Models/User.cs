using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoTaxi.DAL.Models
{
    public class User
    {
        private int userId;
        private string email;
        private string fullName;
        private string password;
        private bool isVisible;
        private int? locationId;
        private Location? location;

        [Key]
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [EmailAddress]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public int? LocationId
        {
            get { return locationId; }
            set { locationId = value; }
        }

        [ForeignKey("LocationId")]
        public Location? Location
        {
            get { return location; }
            set { location = value; }
        }


        public User()
        {
            email = "";
            fullName = "";
            password = "";
            isVisible = false;
            location = null;
        }

        public User(string email, string fullName, string password)
        {
            this.email = email;
            this.fullName = fullName;
            this.password = password;
            isVisible = false;
            location = new Location(90, 90);
        }
    }
}