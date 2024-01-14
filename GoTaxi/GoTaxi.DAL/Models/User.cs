using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoTaxi.DAL.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool IsVisible { get; set; }
        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location? Location { get; set; }

        public User()
        {
            Email = string.Empty;
            FullName = string.Empty;
            Password = string.Empty;
            IsVisible = false;
            Location = null;
        }

        public User(string email, string fullName, string password)
        {
            Email = email;
            FullName = fullName;
            Password = password;
            IsVisible = false;
            Location = new Location(90, 90);
        }
    }
}