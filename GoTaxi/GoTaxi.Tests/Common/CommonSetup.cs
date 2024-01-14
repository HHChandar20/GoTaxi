using GoTaxi.BLL.Services;
using GoTaxi.DAL.Data;
using GoTaxi.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoTaxi.Tests.Common
{
    [SetUpFixture]
    public class CommonSetup
    {
        protected GoTaxiDbContext _context;
        protected ClientRepository _clientRepository;
        protected DriverRepository _driverRepository;

        protected ClientService _clientService;
        protected DriverService _driverService;

        [SetUp]
        public void GlobalSetup()
        {
            // Set up your common context, repositories, and services
            var options = new DbContextOptionsBuilder<GoTaxiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;

            _context = new GoTaxiDbContext(options);
            _clientRepository = new ClientRepository(_context);
            _driverRepository = new DriverRepository(_context);

            _driverService = new DriverService(_driverRepository);
            _clientService = new ClientService(_clientRepository, _driverService);
        }

        [TearDown]
        public void GlobalTearDown()
        {
            _context.Clients.RemoveRange(_context.Clients);
            _context.Users.RemoveRange(_context.Users);
            _context.Drivers.RemoveRange(_context.Drivers);
            _context.Locations.RemoveRange(_context.Locations);
            _context.Destinations.RemoveRange(_context.Destinations);
            _context.SaveChanges();

            _context.Dispose();
        }
    }

}