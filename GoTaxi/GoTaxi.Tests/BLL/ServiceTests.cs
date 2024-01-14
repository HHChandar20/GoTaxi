namespace GoTaxi.Tests.BLL
{
    [TestFixture]
    public class ClientServiceTests : CommonSetup
    {

        [Test]
        public void TestGetClients()
        {
            // Arrange
            List<Client> expectedClients = new List<Client>
            {
                new Client { PhoneNumber = "111-111-1111" },
                new Client { PhoneNumber = "222-222-2222" },
                new Client { PhoneNumber = "333-333-3333" }
            };

            _context.Clients.AddRange(expectedClients);
            _context.SaveChanges();

            // Act
            List<Client> actualClients = _clientService.GetClients();

            // Assert
            Assert.That(actualClients, Is.EqualTo(expectedClients));
        }

        [Test]
        public void TestCheckClientWithExistingClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            _context.Clients.Add(new Client { PhoneNumber = phoneNumber });
            _context.SaveChanges();

            // Act
            bool result = _clientService.CheckClient(phoneNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestCheckClientWithNonExistingClient()
        {
            // Arrange
            string phoneNumber = "999-999-9999";

            // Act
            bool result = _clientService.CheckClient(phoneNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestAuthenticateClientWithValidCredentials()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string password = "password123";

            _context.Clients.Add(new Client { PhoneNumber = phoneNumber, User = new User { Password = password } });
            _context.SaveChanges();

            // Act
            bool result = _clientService.AuthenticateClient(phoneNumber, password);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestAuthenticateClientWithInvalidCredentials()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string validPassword = "password123";
            string invalidPassword = "wrongpassword";

            _context.Clients.Add(new Client { PhoneNumber = phoneNumber, User = new User { Password = validPassword } });
            _context.SaveChanges();

            // Act
            bool result = _clientService.AuthenticateClient(phoneNumber, invalidPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestConvertToClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string fullName = "John Doe";
            string email = "john.doe@example.com";
            string password = "password123";

            // Act
            Client result = _clientService.ConvertToClient(phoneNumber, fullName, email, password);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PhoneNumber, Is.EqualTo(phoneNumber));
            Assert.That(result.User, Is.Not.Null);
            Assert.That(result.User.FullName, Is.EqualTo(fullName));
            Assert.That(result.User.Location, Is.Not.Null);
        }

        [Test]
        public void TestAddClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string fullName = "John Doe";
            string email = "john.doe@example.com";
            string password = "password123";
            Client client = new Client(phoneNumber, new User(email, fullName, password));

            // Act
            _clientService.AddClient(client);

            // Assert
            Client? addedClient = _context.Clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            Assert.That(addedClient, Is.Not.Null);
            Assert.That(addedClient.User!.FullName, Is.EqualTo(fullName));
        }

        [Test]
        public void TestUpdateClientLocation()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            double longitude = -74.0060;
            double latitude = 40.7128;

            _context.Clients.Add(new Client { PhoneNumber = phoneNumber, User = new User { Location = new Location(longitude, latitude) } });

            _context.SaveChanges();

            // Act

            double newLongitude = -24.0060;
            double newLatitude = 10.7128;

            _clientService.UpdateClientLocation(phoneNumber, newLongitude, newLatitude);

            // Assert
            Client updatedClient = _context.Clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber)!;

            Assert.That(updatedClient, Is.Not.Null);
            Assert.That(updatedClient.User!.Location!.Longitude, Is.Not.EqualTo(longitude));
            Assert.That(updatedClient.User.Location.Latitude, Is.Not.EqualTo(latitude));
        }


        [Test]
        public void TestUpdateClientDestination()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            double longitude = -74.0060;
            double latitude = 40.7128;
            string destination = "Some Destination";
            bool visibility = true;

            _context.Clients.Add(new Client
            {
                PhoneNumber = phoneNumber,
                User = new User { Location = new Location(0, 0) },
                Destination = new Destination { Location = new Location(0, 0) }
            });
            _context.SaveChanges();

            // Act
            _clientService.UpdateClientDestination(phoneNumber, destination, longitude, latitude, visibility);

            // Assert
            Client updatedClient = _context.Clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber)!;

            Assert.That(updatedClient, Is.Not.Null);
            Assert.That(updatedClient.Destination!.Name, Is.EqualTo(destination));
            Assert.That(updatedClient.Destination.Location!.Longitude, Is.EqualTo(longitude));
            Assert.That(updatedClient.Destination.Location.Latitude, Is.EqualTo(latitude));
            Assert.That(updatedClient.User!.IsVisible, Is.EqualTo(visibility));
        }

        [Test]
        public void TestClaimClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string plateNumber = "ABC123";
            _context.Clients.Add(new Client { PhoneNumber = phoneNumber });
            _context.Drivers.Add(new Driver { PlateNumber = plateNumber });
            _context.SaveChanges();

            // Act
            _clientService.ClaimClient(plateNumber, phoneNumber);

            // Assert
            Client claimedClient = _context.Clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber)!;

            Assert.That(claimedClient, Is.Not.Null);
            Assert.That(claimedClient.ClaimedBy?.PlateNumber, Is.EqualTo(plateNumber));
        }

        [Test]
        public void TestClientClaimedByWithClaimedClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string plateNumber = "ABC123";
            _context.Clients.Add(new Client { PhoneNumber = phoneNumber, ClaimedBy = new Driver { PlateNumber = plateNumber } });
            _context.SaveChanges();

            // Act
            Driver result = _clientService.ClientClaimedBy(phoneNumber)!;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PlateNumber, Is.EqualTo(plateNumber));
        }

        [Test]
        public void TestClientClaimedByWithUnclaimedClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            _context.Clients.Add(new Client { PhoneNumber = phoneNumber });
            _context.SaveChanges();

            // Act
            Driver result = _clientService.ClientClaimedBy(phoneNumber)!;

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void TestGetClaimedClientWithClaimedClient()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string plateNumber = "ABC123";
            _context.Clients.Add(new Client { PhoneNumber = phoneNumber, User = new User { Location = new Location(0, 0), IsVisible = true }, ClaimedBy = new Driver { PlateNumber = plateNumber, User = new User { Location = new Location(0, 0), IsVisible = false } } });


            _context.SaveChanges();

            // Act
            Client result = _clientService.GetClaimedClient(plateNumber)!;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PhoneNumber, Is.EqualTo(phoneNumber));
        }

        [Test]
        public void TestIsInTheCarWhenInCar()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string plateNumber = "ABC123";


            Driver driver = new Driver { PlateNumber = plateNumber, User = new User { Location = new Location(10, 10) } };
            Client client = new Client
            {
                PhoneNumber = phoneNumber,
                ClaimedBy = driver,
                User = new User { Location = new Location(10, 10) }
            };

            _context.Drivers.Add(driver);
            _context.Clients.Add(client);

            _context.SaveChanges();

            // Act
            bool result = _clientService.IsInTheCar(phoneNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestIsInTheCarWhenNotInCar()
        {
            // Arrange
            string phoneNumber = "111-111-1111";
            string plateNumber = "ABC123";

            Driver driver = new Driver { PlateNumber = plateNumber, User = new User { Location = new Location(10, 10) } };
            Client client = new Client
            {
                PhoneNumber = phoneNumber,
                ClaimedBy = driver,
                User = new User { Location = new Location(0, 0) }
            };

            _context.Drivers.Add(driver);
            _context.Clients.Add(client);

            _context.SaveChanges();

            // Act
            bool result = _clientService.IsInTheCar(phoneNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestGetNearestClients()
        {
            // Arrange
            string plateNumber = "ABC123";

            double longitude = 0;
            double latitude = 0;

            _context.Drivers.Add(new Driver { PlateNumber = plateNumber, User = new User { Location = new Location(longitude, latitude), IsVisible = true } });

            _context.Clients.Add(new Client
            {
                PhoneNumber = "111-111-1111",
                User = new User { Location = new Location(longitude, latitude), IsVisible = true }
            });
            _context.Clients.Add(new Client
            {
                PhoneNumber = "222-222-2222",
                User = new User { Location = new Location(longitude + 2, latitude + 2), IsVisible = true },
            });

            _context.SaveChanges();

            // Act
            List<Client> result = _clientService.GetNearestClients(plateNumber, longitude, latitude);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1)); // Adjust based on your test data
        }
    }


    [TestFixture]
    public class DriverServiceTests : CommonSetup
    {
        [Test]
        public void TestGetDrivers()
        {
            // Arrange
            List<Driver> expectedDrivers = new List<Driver>
            {
                new Driver { PlateNumber = "ABC123" },
                new Driver { PlateNumber = "XYZ456" },
                new Driver { PlateNumber = "123DEF" }
            };

            _context.Drivers.AddRange(expectedDrivers);
            _context.SaveChanges();

            // Act
            List<Driver> actualDrivers = _driverService.GetDrivers();

            // Assert
            Assert.That(actualDrivers, Is.EqualTo(expectedDrivers));
        }

        [Test]
        public void TestCheckDriverWithExistingDriver()
        {
            // Arrange
            string plateNumber = "ABC123";
            _context.Drivers.Add(new Driver { PlateNumber = plateNumber });
            _context.SaveChanges();

            // Act
            bool result = _driverService.CheckDriver(plateNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestCheckDriverWithNonExistingDriver()
        {
            // Arrange
            string plateNumber = "DEF456";

            // Act
            bool result = _driverService.CheckDriver(plateNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestAuthenticateDriverWithValidCredentials()
        {
            // Arrange
            string plateNumber = "ABC123";
            string password = "password123";

            _context.Drivers.Add(new Driver { PlateNumber = plateNumber, User = new User { Password = password } });
            _context.SaveChanges();

            // Act
            bool result = _driverService.AuthenticateDriver(plateNumber, password);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestAuthenticateDriverWithInvalidCredentials()
        {
            // Arrange
            string plateNumber = "ABC123";
            string validPassword = "password123";
            string invalidPassword = "wrongpassword";

            _context.Drivers.Add(new Driver { PlateNumber = plateNumber, User = new User { Password = validPassword } });
            _context.SaveChanges();

            // Act
            bool result = _driverService.AuthenticateDriver(plateNumber, invalidPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestConvertToDriver()
        {
            // Arrange
            string plateNumber = "ABC123";
            string fullName = "John Doe";
            string email = "john.doe@example.com";
            string password = "password123";

            // Act
            Driver result = _driverService.ConvertToDriver(plateNumber, fullName, email, password);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PlateNumber, Is.EqualTo(plateNumber));
            Assert.That(result.User, Is.Not.Null);
            Assert.That(result.User.FullName, Is.EqualTo(fullName));
            Assert.That(result.User.Location, Is.Not.Null);
        }

        [Test]
        public void TestAddDriverWithValidData()
        {
            // Arrange
            string plateNumber = "ABC123";
            string fullName = "John Doe";
            string email = "john.doe@example.com";
            string password = "password123";
            Driver driver = new Driver(plateNumber, new User(email, fullName, password));

            // Act
            _driverService.AddDriver(driver);

            // Assert
            Driver? addedDriver = _context.Drivers.FirstOrDefault(d => d.PlateNumber == plateNumber);

            Assert.That(addedDriver, Is.Not.Null);
            Assert.That(addedDriver.User!.FullName, Is.EqualTo(fullName));
        }

        [Test]
        public void TestUpdateDriverLocation()
        {
            // Arrange
            string plateNumber = "ABC123";
            double longitude = -74.0060;
            double latitude = 40.7128;

            _context.Drivers.Add(new Driver { PlateNumber = plateNumber, User = new User { Location = new Location(longitude, latitude) } });
            _context.SaveChanges();

            // Act
            double newLongitude = -24.0060;
            double newLatitude = 10.7128;

            _driverService.UpdateDriverLocation(plateNumber, newLongitude, newLatitude);

            // Assert
            Driver updatedDriver = _context.Drivers.FirstOrDefault(d => d.PlateNumber == plateNumber)!;

            Assert.That(updatedDriver, Is.Not.Null);
            Assert.That(updatedDriver.User!.Location!.Longitude, Is.Not.EqualTo(longitude));
            Assert.That(updatedDriver.User.Location.Latitude, Is.Not.EqualTo(latitude));
        }

        [Test]
        public void TestUpdateDriverVisibilityWithValidPlateNumber()
        {
            // Arrange
            string plateNumber = "ABC123";
            _context.Drivers.Add(new Driver { PlateNumber = plateNumber, User = new User { IsVisible = true } });
            _context.SaveChanges();

            // Act
            _driverService.UpdateDriverVisibility(plateNumber);

            // Assert
            Driver updatedDriver = _context.Drivers.FirstOrDefault(d => d.PlateNumber == plateNumber)!;
            Assert.That(updatedDriver, Is.Not.Null);
            Assert.That(updatedDriver.User!.IsVisible, Is.EqualTo(true)); // The visibility should not change
        }

        [Test]
        public void TestUpdateDriverVisibilityWithInvalidPlateNumber()
        {
            // Arrange
            string invalidPlateNumber = "InvalidPlateNumber";

            // Act & Assert
            Assert.That(() => _driverService.UpdateDriverVisibility(invalidPlateNumber), Throws.TypeOf<NullReferenceException>());
        }


        [Test]
        public void TestGetNearestDrivers()
        {
            // Arrange
            string plateNumber = "ABC123";
            double longitude = 0;
            double latitude = 0;

            _context.Drivers.Add(new Driver { PlateNumber = plateNumber, User = new User { Location = new Location(longitude, latitude), IsVisible = true } });
            _context.Drivers.Add(new Driver { PlateNumber = "XYZ456", User = new User { Location = new Location(longitude + 0.0000001, latitude + 0.0000001), IsVisible = true } });
            _context.SaveChanges();

            // Act
            List<Driver> result = _driverService.GetNearestDrivers(plateNumber, longitude, latitude);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1)); // Adjust based on your test data
        }

    }


    [TestFixture]
    public class DistanceCalculatorTests
    {

        [Test]
        public void TestCalculateDistance()
        {
            // Arrange
            Location location1 = new Location(-74.0060, 40.7128); // Example location
            Location location2 = new Location(-118.2437, 34.0522); // Example location

            double expectedResult = 3935.746;

            // Act
            double result = DistanceCalculator.CalculateDistance(location1, location2);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult).Within(0.1)); // Adjust the delta based on your expected precision
        }

        [Test]
        public void TestDegreesToRadiansWithNinetyResultsPiDividedByTwo()
        {
            // Arrange
            double expectedResult = Math.PI / 2;

            // Act
            double result = DistanceCalculator.DegreesToRadians(90);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult).Within(0.0001)); // Adjust the delta based on your expected precision
        }
    }
}