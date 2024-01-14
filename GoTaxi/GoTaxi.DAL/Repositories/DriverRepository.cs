using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GoTaxi.DAL.Repositories
{
    public class DriverRepository
    {
        private readonly GoTaxiDbContext _context;

        public DriverRepository(GoTaxiDbContext context)
        {
            _context = context;
        }

        public List<Driver> GetAllDrivers()
        {
            return _context.Drivers.ToList();
        }

        public List<Driver> GetAllDriversExceptCurrent(string plateNumber)
        {
            return _context.Drivers.Where(driver => driver.PlateNumber != plateNumber).ToList();
        }

        public Driver GetDriverByPlateNumber(string plateNumber)
        {
            return _context.Drivers.FirstOrDefault(driver => driver.PlateNumber == plateNumber);
        }

        public void AddDriver(Driver newDriver)
        {
            _context.Drivers.Add(newDriver);
            _context.SaveChanges();
        }

        public void UpdateDriver(Driver updatedDriver)
        {
            var existingDriver = _context.Drivers.FirstOrDefault(driver => driver.PlateNumber == updatedDriver.PlateNumber);

            if (existingDriver != null)
            {
                existingDriver.Email = updatedDriver.Email;
                existingDriver.FullName = updatedDriver.FullName;
                existingDriver.Password = updatedDriver.Password;
                existingDriver.Longitude = updatedDriver.Longitude;
                existingDriver.Latitude = updatedDriver.Latitude;

                _context.SaveChanges();
            }
        }
    }
}