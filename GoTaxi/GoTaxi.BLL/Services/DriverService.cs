using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

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
            List<Driver> drivers = repositoryInstance.GetDrivers();

            foreach (Driver driver in drivers)
            {
                if (driver.PlateNumber == plateNumber)
                {
                    return true;
                }
            }

            return false;
        }

        public Driver ConvertToDriver(string plateNumber, string username, string email, string password)
        {
            Driver driver = new Driver();

            driver.PlateNumber = plateNumber;
            driver.Username = username;
            driver.Email = email;
            driver.Password = password;

            return driver;
        }

        public void AddDriver(string plateNumber, string username, string email, string password)
        {
            repositoryInstance.AddDriver(ConvertToDriver(plateNumber, username, email, password));
        }
    }
}