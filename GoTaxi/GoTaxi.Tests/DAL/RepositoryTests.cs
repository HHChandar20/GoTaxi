using Microsoft.EntityFrameworkCore;
using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;

namespace GoTaxi.Tests.GoTaxi.DAL.NUnitTests
{
    [TestFixture]
    public class ClientRepositoryTests
    {
        private GoTaxiDbContext _context;
        private IClientRepository _clientRepository;

        [SetUp]
        public void Setup()
        {
            // Set up an in-memory database for testing
            var options = new DbContextOptionsBuilder<GoTaxiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new GoTaxiDbContext(options);
            _clientRepository = new ClientRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Clients.RemoveRange(_context.Clients);
            _context.Users.RemoveRange(_context.Users);
            _context.Drivers.RemoveRange(_context.Drivers);
            _context.Locations.RemoveRange(_context.Locations);
            _context.Destinations.RemoveRange(_context.Destinations);
            _context.SaveChanges();

            _context.Dispose();
        }

        [Test]
        public void GetAllClients_ReturnsAllClients()
        {
            // Arrange
            var clients = new[]
            {
                new Client { PhoneNumber = "111-111-1111" },
                new Client { PhoneNumber = "222-222-2222" },
                new Client { PhoneNumber = "333-333-3333" }
            };
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // Act
            var result = _clientRepository.GetAllClients();

            // Assert
            Assert.That(result.Count, Is.EqualTo(clients.Length));

            foreach (var client in clients)
            {
                Assert.That(result, Does.Contain(client));
            }
        }

        [Test]
        public void GetAllClientsExceptCurrent_ReturnsNull()
        {
            // Arrange
            Client client = new Client { PhoneNumber = "111-111-1111" };

            _context.Clients.Add(client);
            _context.SaveChanges();

            List<Client> expectedResult = new List<Client>();

            // Act
            List<Client> actualResult = _clientRepository.GetAllClientsExceptCurrent("111-111-1111");

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void AddClient_AddsClientToDatabase()
        {
            // Arrange
            var newClient = new Client { PhoneNumber = "444-444-4444" };

            // Act
            _clientRepository.AddClient(newClient);

            // Assert
            var result = _context.Clients.FirstOrDefault(c => c.PhoneNumber == "444-444-4444");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PhoneNumber, Is.EqualTo(newClient.PhoneNumber));
        }

        [Test]
        public void UpdateClient_UpdatesExistingClientInDatabase()
        {
            // Arrange
            User user = new User("email123", "fullNamE", "Passiword");

            var existingClient = new Client("0888", user);
            existingClient.ClientId = 1;

            _context.Clients.Add(existingClient);
            _context.SaveChanges();

            User updatedUser = user;
            updatedUser.Email = "email";
            updatedUser.FullName = "fullName";
            updatedUser.Password = "Password";

            var updatedClient = existingClient;
            updatedClient.User = updatedUser;

            // Act
            _clientRepository.UpdateClient(updatedClient);

            // Assert
            var result = _context.Clients.Include(c => c.User).FirstOrDefault(c => c.PhoneNumber == "0888");

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.User!.Email, Is.EqualTo(updatedClient.User!.Email));
                Assert.That(result.User!.FullName, Is.EqualTo(updatedClient.User.FullName));
                Assert.That(result.User!.Password, Is.EqualTo(updatedClient.User.Password));
            });
        }
    }
}