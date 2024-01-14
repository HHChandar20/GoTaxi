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

        public void SetDriverVisibility(string plateNumber, bool visibility)
        {
            Driver currentDriver = _repository.GetDriverByPlateNumber(plateNumber);

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

        public bool AuthenticateDriver(string plateNumber, string password)
        {
            Driver? driver = _repository.GetAllDriversWithUsers().FirstOrDefault(driver => driver.PlateNumber == plateNumber && driver.User?.Password == password);

            if (driver == null)
            {
                return false;
            }

            return true;
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
        public void AddDriver(Driver driver)
        {
            _repository.AddDriver(driver);
        }

        public void UpdateDriver(string plateNumber, string fullName, string email, string password)
        {
            _repository.UpdateDriver(ConvertToDriver(plateNumber, fullName, email, password));
        }
        public void UpdateDriver(Driver driver)
        {
            _repository.UpdateDriver(driver);
        }

        public void UpdateDriverVisibility(string plateNumber)
        {

            Driver driver = _repository.GetDriverByPlateNumber(plateNumber);

            _repository.UpdateDriverVisibility(driver);
        }

        public void UpdateDriverVisibility(Driver driver)
        {
            _repository.UpdateDriverVisibility(driver);
        }

        public void UpdateDriverLocation(string plateNumber, double newLongitude, double newLatitude)
        {
            Driver driver = _repository.GetDriverByPlateNumber(plateNumber);

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

        public List<Driver> GetNearestDrivers(string plateNumber, double currentDriverLongitude, double currentDriverLatitude)
        {
            Driver currentDriver = GetDriverByPlateNumber(plateNumber);

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
                DistanceCalculator.CalculateDistance(currentLocation, driver.User.Location!) <= 6000)
            .OrderBy(driver =>
                DistanceCalculator.CalculateDistance(currentLocation, driver.User!.Location!))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 drivers, otherwise, get all available drivers.
            int count = Math.Min(filteredLocations.Count, 10);

            List<Driver> nearestLocations = filteredLocations.GetRange(0, count);

            return nearestLocations;
        }

    }
}