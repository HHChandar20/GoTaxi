using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;

namespace GoTaxi.BLL.Services
{
    public class DriverService : IDriverService
    {
        private readonly DriverRepository _repository;

        public DriverService(DriverRepository repository)
        {
            _repository = repository;
        }

        public void SetDriverVisibility(Driver currentDriver, bool visibility)
        {
            currentDriver.User!.IsVisible = visibility;
            _repository.UpdateDriverVisibility(currentDriver);
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

        public Driver? AuthenticateDriver(string plateNumber, string password)
        {
            return _repository.GetAllDriversWithUsers().First(driver => driver.PlateNumber == plateNumber && driver.User!.Password == password);
        }

        public Driver ConvertToDriver(string plateNumber, string fullName, string email, string password)
        {
            User user = new User(email, fullName, password);
            Driver driver = new Driver(plateNumber, user);

            driver.User!.IsVisible = true;

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
        public void UpdateDriver(Driver driver)
        {
            _repository.UpdateDriver(driver);
        }

        public void UpdateDriverVisibility(Driver driver)
        {
            _repository.UpdateDriverVisibility(driver);
        }

        public void UpdateDriverLocation(Driver driver, double newLongitude, double newLatitude)
        {
            if (driver != null && driver.User != null && driver.User.Location != null)
            {
                driver.User.Location.Longitude = newLongitude;
                driver.User.Location.Latitude = newLatitude;

                _repository.UpdateDriver(driver);
            }
            else
            {
                Console.WriteLine("Error updating driver location");
            }
        }

        public List<Driver> GetNearestDrivers(Driver currentDriver, double currentDriverLongitude, double currentDriverLatitude)
        {
            Location currentLocation = new Location(currentDriverLongitude, currentDriverLatitude);

            if (currentDriver.User!.IsVisible == false)
            {
                return new List<Driver>();
            }

            List<Driver> drivers = _repository.GetAllDriversExceptCurrentWithUsers(currentDriver.PlateNumber);

            if (drivers == null)
            {
                return new List<Driver>(); // Return an empty list if there are no other drivers or only the current driver.
            }

            List<Driver> filteredLocations = drivers
            .Where(driver =>
                driver.User!.IsVisible == true &&
                CalculateDistance(currentLocation, driver.User.Location!) <= 6000)
            .OrderBy(driver =>
                CalculateDistance(currentLocation, driver.User!.Location!))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 drivers, otherwise, get all available drivers.
            int count = Math.Min(filteredLocations.Count, 10);

            List<Driver> nearestLocations = filteredLocations.GetRange(0, count);

            return nearestLocations;
        }


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