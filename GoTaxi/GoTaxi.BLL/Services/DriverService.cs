using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;

namespace GoTaxi.BLL.Services
{
    public class DriverService : IDriverService
    {
        private static DriverRepository repositoryInstance = DriverRepository.GetInstance();

        public List<Driver> GetDrivers()
        {
            return repositoryInstance.GetDrivers();
        }

        public bool CheckDriver(string plateNumber)
        {
            List<Driver> drivers = repositoryInstance.GetAllDrivers();

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

        public Driver ConvertToDriver(string plateNumber, string fullName, string email, string password)
        {
            Driver driver = new Driver();

            driver.PlateNumber = plateNumber;
            driver.Email = email;
            driver.FullName = fullName;
            driver.Password = password;

            return driver;
        }

        public void AddDriver(string plateNumber, string fullName, string email, string password)
        {
            repositoryInstance.AddDriver(ConvertToDriver(plateNumber, fullName, email, password));
        }

        public void UpdateDriver(string plateNumber, string fullName, string email, string password)
        {
            repositoryInstance.UpdateDriver(ConvertToDriver(plateNumber, fullName, email, password));
        }
    }
}