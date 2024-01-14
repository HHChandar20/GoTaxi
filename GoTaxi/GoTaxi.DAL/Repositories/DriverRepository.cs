using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;

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

        public List<Driver> GetAllDriversWithUsers()
        {
            return _context.Drivers
                .Include(d => d.User)
                    .ThenInclude(u => u!.Location)
                .ToList();
        }

        public List<Driver> GetAllDriversExceptCurrentWithUsers(string plateNumber)
        {
            return _context.Drivers
                .Include(d => d.User)
                    .ThenInclude(u => u!.Location)
                .Where(driver => driver.PlateNumber != plateNumber)
                .ToList();
        }

        public Driver GetDriverByPlateNumber(string plateNumber)
        {
            return _context.Drivers.Include(d => d.User).ThenInclude(u => u!.Location).First(driver => driver.PlateNumber == plateNumber);
        }

        public void AddDriver(Driver newDriver)
        {
            _context.Drivers.Add(newDriver);
            _context.SaveChanges();
        }

        public void UpdateDriver(Driver updatedDriver)
        {
            Driver existingDriver = GetDriverByPlateNumber(updatedDriver.PlateNumber);

            if (existingDriver != null)
            {
                _context.Entry(existingDriver).CurrentValues.SetValues(updatedDriver);
                _context.SaveChanges();
            }
        }
    }
}