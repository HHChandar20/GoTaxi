namespace GoTaxi.PL.Models
{
    public class AccountViewModel
    {
        private int _id;
        private string _username;
        private string password;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }

}