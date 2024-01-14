using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GoTaxi.DAL.Repositories
{
    // Repository class for handling CRUD operations related to Driver entities
    public class DriverRepository
    {
        // The DbContext used for interacting with the database
        private readonly GoTaxiDbContext _context;

        // Constructor that initializes the repository with a DbContext
        public DriverRepository(GoTaxiDbContext context)
        {
            _context = context;
        }

        // Retrieve all drivers from the database
        public List<Driver> GetAllDrivers()
        {
            return _context.Drivers.ToList();
        }

        // Retrieve all drivers with associated users and their locations
        public List<Driver> GetAllDriversWithUsers()
        {
            return _context.Drivers
                .Include(d => d.User)
                    .ThenInclude(u => u!.Location)
                .ToList();
        }

        // Retrieve all drivers except the one with the provided plate number, with associated users and locations
        public List<Driver> GetAllDriversExceptCurrentWithUsers(string plateNumber)
        {
            return _context.Drivers
                .Include(d => d.User)
                    .ThenInclude(u => u!.Location)
                .Where(driver => driver.PlateNumber != plateNumber)
                .ToList();
        }

        // Retrieve a driver by their plate number with associated user and location information
        public Driver GetDriverByPlateNumber(string plateNumber)
        {
            return _context.Drivers.Include(d => d.User).ThenInclude(u => u!.Location).FirstOrDefault(driver => driver.PlateNumber == plateNumber)!;
        }

        // Add a new driver to the database
        public void AddDriver(Driver newDriver)
        {
            _context.Drivers.Add(newDriver);
            _context.SaveChanges();
        }

        // Update an existing driver in the database
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