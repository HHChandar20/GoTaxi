using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;

namespace GoTaxi.BLL.Services
{
    public class DriverService : IDriverService
    {
        private static Driver currentDriver = new Driver();
        private readonly DriverRepository _repository;

        public DriverService(DriverRepository repository)
        {
            _repository = repository;
        }

        public Driver GetCurrentDriver()
        {
            return currentDriver;
        }

        public List<Driver> GetDrivers()
        {
            return _repository.GetAllDrivers();
        }

        public Driver GetDriverByPlateNumber(string plateNumber)
        {
            return _repository.GetDriverByPlateNumber(plateNumber);
        }

        public bool CheckDriver(string plateNumber)
        {
            List<Driver> drivers = _repository.GetAllDrivers();

            if (drivers == null)
            {
                return false;
            }

            foreach (Driver driver in drivers)
            {
                if (driver.PlateNumber == plateNumber)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AuthenticateDriver(string plateNumber, string password)
        {
            List<Driver> drivers = _repository.GetAllDrivers();

            if (drivers == null)
            {
                return false;
            }

            foreach (Driver driver in drivers)
            {
                if (driver.PlateNumber == plateNumber && driver.Password == password)
                {
                    currentDriver = driver;
                    return true;
                }
            }

            return false;
        }

        public Driver ConvertToDriver(string plateNumber, string fullName, string email, string password)
        {
            Driver driver = new Driver();

            driver.PlateNumber = plateNumber;
            driver.Email = email;
            driver.FullName = fullName;
            driver.Password = password;
            driver.Longitude = 1.1000;
            driver.Latitude = 1.1000;

            return driver;
        }

        public void AddDriver(string plateNumber, string fullName, string email, string password)
        {
            _repository.AddDriver(ConvertToDriver(plateNumber, fullName, email, password));
        }

        public void UpdateDriver(string plateNumber, string fullName, string email, string password)
        {
            _repository.UpdateDriver(ConvertToDriver(plateNumber, fullName, email, password));
        }

        public void UpdateCurrentDriverLocation(double longitude, double latitude)
        {
            currentDriver.Longitude = longitude;
            currentDriver.Latitude = latitude;

            _repository.UpdateDriver(currentDriver);
        }


        public List<Driver> GetNearestDrivers(double currentDriverLongitude, double currentDriverLatitude)
        {
            List<Driver> drivers = _repository.GetAllDriversExceptCurrent(currentDriver.PlateNumber);

            if (drivers == null)
            {
                return new List<Driver>(); // Return an empty list if there are no other drivers or only the current driver.
            }

            drivers.Remove(currentDriver);

            List<Driver> filteredLocations = drivers
            .Where(driver =>
                CalculateDistance(currentDriverLongitude, currentDriverLatitude, driver.Longitude, driver.Latitude) <= 60)
            .OrderBy(client =>
                CalculateDistance(currentDriverLongitude, currentDriverLatitude, client.Longitude, client.Latitude))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 drivers, otherwise, get all available drivers.
            int count = Math.Min(filteredLocations.Count, 10);
            List<Driver> nearestLocations = filteredLocations.GetRange(0, count);

            return nearestLocations;
        }


        public static double CalculateDistance(double longitude1, double latitude1, double longitude2, double latitude2)
        {
            double dLat = DegreesToRadians(latitude2 - latitude1);
            double dLon = DegreesToRadians(longitude2 - longitude1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(DegreesToRadians(latitude1)) * Math.Cos(DegreesToRadians(latitude2)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371 * c; // Distance in kilometers // 6371 - Earth radius in kilometers
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

    }
}