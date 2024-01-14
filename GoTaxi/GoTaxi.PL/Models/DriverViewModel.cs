namespace GoTaxi.PL.Models
{
    public class DriverViewModel
    {
        private string _plateNumber;
        private string _email;
        private string _fullName;
        private string _password;
        private double _longitude;
        private double _latitude;

        public string PlateNumber
        {
            get { return _plateNumber; }
            set { _plateNumber = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }


        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }


        public DriverViewModel()
        {
            _plateNumber = "NotDefined";
            _email = "NotDefined";
            _fullName = "NotDefined";
            _password = "NotDefined";
            _longitude = 0.1;
            _latitude = 0.1;
        }

    }

}