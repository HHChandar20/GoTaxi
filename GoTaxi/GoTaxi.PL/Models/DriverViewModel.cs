namespace GoTaxi.PL.Models
{
    public class DriverViewModel
    {
        private string _plateNumber;
        private string _fullName;
        private string _email;
        private string _password;

        public string PlateNumber
        {
            get { return _plateNumber; }
            set { _plateNumber = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
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
            _fullName = "NotDefined";
            _email = "NotDefined";
            _password = "NotDefined";
        }

    }

}