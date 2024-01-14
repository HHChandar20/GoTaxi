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
            _repository.UpdateDriver(currentDriver);
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
            return _repository.GetAllDrivers()?.Any(driver => driver.PlateNumber == plateNumber) ?? false;
        }

        public bool AuthenticateDriver(string plateNumber, string password)
        {
            return _repository.GetAllDriversWithUsers()
                .Any(driver => driver.PlateNumber == plateNumber && driver.User?.Password == password);
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

            _repository.UpdateDriver(driver);
        }

        public void UpdateDriverVisibility(Driver driver)
        {
            _repository.UpdateDriver(driver);
        }

        public void UpdateDriverLocation(string plateNumber, double longitude, double latitude)
        {
            Driver driver = _repository.GetDriverByPlateNumber(plateNumber);

            if (driver != null && driver.User != null && driver.User.Location != null)
            {
                driver.User.Location.Longitude = longitude;
                driver.User.Location.Latitude = latitude;

                _repository.UpdateDriver(driver);
            }
            else
            {
                Console.WriteLine("Error updating driver location");
            }
        }

        public List<Driver> GetNearestDrivers(string plateNumber, double longitude, double latitude)
        {
            Driver currentDriver = GetDriverByPlateNumber(plateNumber);
            Location currentLocation = new Location(longitude, latitude);

            List<Driver> drivers = _repository.GetAllDriversExceptCurrentWithUsers(currentDriver.PlateNumber);

            if (currentDriver.User!.IsVisible == false || drivers == null)
            {
                return new List<Driver>();
            }


            List<Driver> filteredDrivers = drivers
            .Where(driver =>
                driver.User!.IsVisible == true &&
                DistanceCalculator.CalculateDistance(currentLocation, driver.User.Location!) <= DistanceCalculator.Range) // Max distance 60 km
            .OrderBy(driver =>
                DistanceCalculator.CalculateDistance(currentLocation, driver.User!.Location!))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 drivers, otherwise, get all available drivers.
            int count = Math.Min(filteredDrivers.Count, 10);

            List<Driver> nearestLocations = filteredDrivers.GetRange(0, count);

            return nearestLocations;
        }

    }
}