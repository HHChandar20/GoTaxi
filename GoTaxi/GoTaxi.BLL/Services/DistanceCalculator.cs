using GoTaxi.DAL.Models;

namespace GoTaxi.BLL.Services
{
    // Service class for calculating distances between locations
    public class DistanceCalculator
    {
        // Earth's radius in kilometers (used for distance calculation)
        public const int EarthRadiusKilometers = 6371;

        // Maximum allowed distance for various purposes
        public const double MaxDistanceKilometers = 60;
        public const double MaxCarDistanceKilometers = 0.033;


        // Calculate the haversine distance between two locations
        public static double CalculateDistance(Location location1, Location location2)
        {
            // Convert latitude and longitude differences to radians
            double deltaLatitude = DegreesToRadians(location2.Latitude - location1.Latitude);
            double deltaLongitude = DegreesToRadians(location2.Longitude - location1.Longitude);

            // Haversine formula
            double haversineA = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                        Math.Cos(DegreesToRadians(location1.Latitude)) * Math.Cos(DegreesToRadians(location2.Latitude)) *
                        Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

            // Calculate the central angle using the haversine formula
            double haversineC = 2 * Math.Atan2(Math.Sqrt(haversineA), Math.Sqrt(1 - haversineA));

            // Calculate the distance using the haversine formula and Earth's radius
            return EarthRadiusKilometers * haversineC; // Distance in kilometers
        }

        // Convert degrees to radians
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}