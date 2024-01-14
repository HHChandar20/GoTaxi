namespace GoTaxi.PL.Models
{
    public class DriverViewModel
    {
        private string _plateNumber;
        private string _username;
        private string _email;
        private string _password;

        public string PlateNumber
        {
            get { return _plateNumber; }
            set { _plateNumber = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }


        DriverViewModel()
        {
            _plateNumber = "NotDefined";
            _username = "NotDefined";
            _email = "NotDefined";
            _password = "NotDefined";
        }

    }

}