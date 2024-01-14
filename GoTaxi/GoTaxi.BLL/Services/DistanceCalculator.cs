using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Services
{
    public class DistanceCalculator
    {
        public static double CalculateDistance(Location location1, Location location2)
        {
            double dLat = DegreesToRadians(location2.Latitude - location1.Latitude);
            double dLon = DegreesToRadians(location2.Longitude - location1.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(DegreesToRadians(location1.Latitude)) * Math.Cos(DegreesToRadians(location2.Latitude)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371 * c; // Distance in kilometers (6371 - Earth radius in kilometers)
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}