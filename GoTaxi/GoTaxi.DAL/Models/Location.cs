namespace GoTaxi.DAL.Models
{
    public class Location
    {
        private int locationId;
        private double longitude;
        private double latitude;

        public int LocationId
        {
            get { return locationId; }
            set { locationId = value; }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public Location(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}