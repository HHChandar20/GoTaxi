using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GoTaxi.Tests.DAL
{
    [TestFixture]
    public class ClientRepositoryTests : CommonSetup
    {

        [Test]
        public void TestGetAllClients()
        {
            // Arrange
            List<Client> clients = new List<Client>
            {
                new Client { PhoneNumber = "111-111-1111" },
                new Client { PhoneNumber = "222-222-2222" },
                new Client { PhoneNumber = "333-333-3333" }
            };
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // Act
            List<Client> result = _clientRepository.GetAllClients();

            // Assert
            Assert.That(result.Count, Is.EqualTo(clients.Count));

            foreach (Client client in clients)
            {
                Assert.That(result, Does.Contain(client));
            }
        }

        [Test]
        public void TestGetAllClientsExceptCurrentReturnsNull()
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
        public void TestAddClient()
        {
            // Arrange
            Client newClient = new Client { PhoneNumber = "444-444-4444" };

            // Act
            _clientRepository.AddClient(newClient);

            // Assert
            Client result = _context.Clients.FirstOrDefault(c => c.PhoneNumber == "444-444-4444")!;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PhoneNumber, Is.EqualTo(newClient.PhoneNumber));
        }

        [Test]
        public void TestUpdateClient()
        {
            // Arrange
            User user = new User("email123", "fullNamE", "Passiword");

            Client existingClient = new Client("0888", user);
            existingClient.ClientId = 1;

            _context.Clients.Add(existingClient);
            _context.SaveChanges();

            User updatedUser = user;
            updatedUser.Email = "email";
            updatedUser.FullName = "fullName";
            updatedUser.Password = "Password";

            Client updatedClient = existingClient;
            updatedClient.User = updatedUser;

            // Act
            _clientRepository.UpdateClient(updatedClient);

            // Assert
            Client result = _context.Clients.Include(c => c.User).FirstOrDefault(c => c.PhoneNumber == "0888")!;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.User!.Email, Is.EqualTo(updatedClient.User!.Email));
            Assert.That(result.User!.FullName, Is.EqualTo(updatedClient.User.FullName));
            Assert.That(result.User!.Password, Is.EqualTo(updatedClient.User.Password));
        }

        [Test]
        public void TestGetAllClientsWithUsers()
        {
            // Arrange
            List<Client> clients = new List<Client>
            {
                new Client { PhoneNumber = "111-111-1111", User = new User { Email = "user1@example.com" } },
                new Client { PhoneNumber = "222-222-2222", User = new User { Email = "user2@example.com" } },
                new Client { PhoneNumber = "333-333-3333", User = new User { Email = "user3@example.com" } }
            };
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // Act
            List<Client> result = _clientRepository.GetAllClientsWithUsers();

            // Assert
            Assert.That(result.Count, Is.EqualTo(clients.Count));
            foreach (Client client in clients)
            {
                Assert.That(result, Does.Contain(client));
                Assert.That(result.Find(c => c.PhoneNumber == client.PhoneNumber)?.User, Is.Not.Null);
            }
        }

        [Test]
        public void TestGetAllClientsWithDestinations()
        {
            // Arrange
            List<Client> clients = new List<Client>
            {
                new Client { PhoneNumber = "111-111-1111", Destination = new Destination { Name = "Destination1" } },
                new Client { PhoneNumber = "222-222-2222", Destination = new Destination { Name = "Destination2" } },
                new Client { PhoneNumber = "333-333-3333", Destination = new Destination { Name = "Destination3" } }
            };
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // Act
            List<Client> result = _clientRepository.GetAllClientsWithDestinations();

            // Assert
            Assert.That(result.Count, Is.EqualTo(clients.Count));
            foreach (Client client in clients)
            {
                Assert.That(result, Does.Contain(client));
                Assert.That(result.Find(c => c.PhoneNumber == client.PhoneNumber)?.Destination, Is.Not.Null);
            }
        }

        [Test]
        public void TestGetAllClientsWithClaimedBy()
        {
            // Arrange
            List<Client> clients = new List<Client>
            {
                new Client { PhoneNumber = "111-111-1111", ClaimedBy = new Driver { PlateNumber = "Driver1" } },
                new Client { PhoneNumber = "222-222-2222", ClaimedBy = new Driver { PlateNumber = "Driver2" } },
                new Client { PhoneNumber = "333-333-3333", ClaimedBy = new Driver { PlateNumber = "Driver3" } }
            };
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // Act
            List<Client> result = _clientRepository.GetAllClientsWithClaimedBy();

            // Assert
            Assert.That(result.Count, Is.EqualTo(clients.Count));
            foreach (Client client in clients)
            {
                Assert.That(result, Does.Contain(client));
                Assert.That(result.Find(c => c.PhoneNumber == client.PhoneNumber)?.ClaimedBy, Is.Not.Null);
            }
        }

        [Test]
        public void TestGetClientByPhoneNumberWithExistingClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            Client client = new Client { PhoneNumber = phoneNumber, User = new User { Email = "user@example.com" } };
            _context.Clients.Add(client);
            _context.SaveChanges();

            // Act
            Client result = _clientRepository.GetClientByPhoneNumber(phoneNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PhoneNumber, Is.EqualTo(phoneNumber));
            Assert.That(result.User, Is.Not.Null);
        }

        [Test]
        public void TestGetClientByPhoneNumberWithNonExistingClient()
        {
            // Arrange
            string nonExistingPhoneNumber = "999-999-9999";

            // Act
            Client result = _clientRepository.GetClientByPhoneNumber(nonExistingPhoneNumber);

            // Assert
            Assert.That(result, Is.Null);
        }

    }


    [TestFixture]
    public class DriverRepositoryTests : CommonSetup
    {
        [Test]
        public void TestGetAllDrivers()
        {
            // Arrange
            List<Driver> drivers = new List<Driver>
            {
                new Driver { PlateNumber = "ABC123" },
                new Driver { PlateNumber = "XYZ789" },
                new Driver { PlateNumber = "123456" }
            };
            _context.Drivers.AddRange(drivers);
            _context.SaveChanges();

            // Act
            List<Driver> result = _driverRepository.GetAllDrivers();

            // Assert
            Assert.That(result.Count, Is.EqualTo(drivers.Count));

            foreach (Driver driver in drivers)
            {
                Assert.That(result, Does.Contain(driver));
            }
        }

        [Test]
        public void TestGetAllDriversWithUsers()
        {
            // Arrange
            List<Driver> drivers = new List<Driver>
            {
                new Driver { PlateNumber = "ABC123", User = new User { Email = "user1@example.com" } },
                new Driver { PlateNumber = "XYZ789", User = new User { Email = "user2@example.com" } },
                new Driver { PlateNumber = "123456", User = new User { Email = "user3@example.com" } }
            };
            _context.Drivers.AddRange(drivers);
            _context.SaveChanges();

            // Act
            List<Driver> result = _driverRepository.GetAllDriversWithUsers();

            // Assert
            Assert.That(result.Count, Is.EqualTo(drivers.Count));

            foreach (Driver driver in drivers)
            {
                Assert.That(result, Does.Contain(driver));
                Assert.That(result.Find(d => d.PlateNumber == driver.PlateNumber)?.User, Is.Not.Null);
            }
        }

        [Test]
        public void TestGetAllDriversExceptCurrentWithUsers()
        {
            // Arrange
            Driver driver = new Driver { PlateNumber = "123456", User = new User { Email = "user3@example.com", Location = new Location(0, 0) } };

            _context.Drivers.Add(driver);
            _context.SaveChanges();

            List<Driver> expectedResult = new List<Driver>();

            // Act
            List<Driver> actualResult = _driverRepository.GetAllDriversExceptCurrentWithUsers("123456");

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestAddDriver()
        {
            // Arrange
            Driver newDriver = new Driver { PlateNumber = "DEF456" };

            // Act
            _driverRepository.AddDriver(newDriver);

            // Assert
            Driver result = _context.Drivers.FirstOrDefault(d => d.PlateNumber == "DEF456")!;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PlateNumber, Is.EqualTo(newDriver.PlateNumber));
        }

        [Test]
        public void TestUpdateDriver()
        {
            // Arrange
            User user = new User("email123", "fullNamE", "Passiword");

            Driver existingDriver = new Driver("ABC123", user);
            existingDriver.DriverId = 1;

            _context.Drivers.Add(existingDriver);
            _context.SaveChanges();

            User updatedUser = user;
            updatedUser.Email = "email";
            updatedUser.FullName = "fullName";
            updatedUser.Password = "Password";

            Driver updatedDriver = existingDriver;
            updatedDriver.User = updatedUser;

            // Act
            _driverRepository.UpdateDriver(updatedDriver);

            // Assert
            Driver result = _context.Drivers.Include(d => d.User).FirstOrDefault(d => d.PlateNumber == "ABC123")!;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.User!.Email, Is.EqualTo(updatedDriver.User!.Email));
            Assert.That(result.User!.FullName, Is.EqualTo(updatedDriver.User.FullName));
            Assert.That(result.User!.Password, Is.EqualTo(updatedDriver.User.Password));
        }

        [Test]
        public void TestGetDriverByPlateNumberWithExistingDriver()
        {
            // Arrange
            string plateNumber = "ABC123";
            Driver driver = new Driver { PlateNumber = plateNumber, User = new User { Email = "user@example.com" } };
            _context.Drivers.Add(driver);
            _context.SaveChanges();

            // Act
            Driver result = _driverRepository.GetDriverByPlateNumber(plateNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PlateNumber, Is.EqualTo(plateNumber));
            Assert.That(result.User, Is.Not.Null);
        }

        [Test]
        public void TestGetDriverByPlateNumberWithNonExistingDriver()
        {
            // Arrange
            string nonExistingPlateNumber = "ZZZ999";

            // Act
            Driver result = _driverRepository.GetDriverByPlateNumber(nonExistingPlateNumber);

            // Assert
            Assert.That(result, Is.Null);
        }
    }

}