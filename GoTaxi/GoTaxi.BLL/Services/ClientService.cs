using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;

namespace GoTaxi.BLL.Services
{
    public class ClientService : IClientService
    {
        private readonly ClientRepository _repository;
        private readonly IDriverService _driverService;

        public ClientService(ClientRepository repository, IDriverService driverService)
        {
            _repository = repository;
            _driverService = driverService;
        }

        public List<Client> GetClients()
        {
            return _repository.GetAllClients();
        }
        public Client GetClientByPhoneNumber(string phoneNumber)
        {
            return _repository.GetClientByPhoneNumber(phoneNumber);
        }

        public bool CheckClient(string phoneNumber)
        {
            List<Client> clients = _repository.GetAllClients();

            if (clients == null)
            {
                return false;
            }

            foreach (Client client in clients)
            {
                if (client.PhoneNumber == phoneNumber)
                {
                    return true;
                }
            }

            return false;
        }

        public Client? AuthenticateClient(string phoneNumber, string password)
        {
            return _repository.GetAllClientsWithUsers().First(client => client.PhoneNumber == phoneNumber && client.User!.Password == password);
        }

        public Client ConvertToClient(string phoneNumber, string fullName, string email, string password)
        {
            User user = new User(email, fullName, password);
            Client client = new Client(phoneNumber, user);

            return client;
        }

        public void AddClient(string phoneNumber, string fullName, string email, string password)
        {
            _repository.AddClient(ConvertToClient(phoneNumber, fullName, email, password));
        }

        public void UpdateClient(string phoneNumber, string fullName, string email, string password)
        {
            _repository.UpdateClient(ConvertToClient(phoneNumber, fullName, email, password));
        }

        public void UpdateClientLocation(Client client, double newLongitude, double newLatitude)
        {
            if (client != null && client.User != null && client.User.Location != null)
            {
                client.User.Location.Longitude = newLongitude;
                client.User.Location.Latitude = newLatitude;

                _repository.UpdateClient(client);
            }
            else
            {
                Console.WriteLine("Error updating client location");
            }
        }

        public void UpdateClientDestination(Client client, string? newDestination, double newLongitude, double newLatitude, bool newVisibility)
        {
            if (client != null && client.Destination != null)
            {
                Driver driver;

                if (client.ClaimedBy != null)
                {
                    driver = _driverService.GetDriverByPlateNumber(client.ClaimedBy.PlateNumber);
                    driver.User!.IsVisible = !newVisibility;

                    if (!newVisibility)
                    {
                        // Client canceled the request
                        _driverService.UpdateDriverVisibility(driver);
                        client.ClaimedBy = null;
                    }
                }

                client.Destination.Name = newDestination;
                client.Destination.Location!.Longitude = newLongitude;
                client.Destination.Location.Latitude = newLatitude;
                client.User!.IsVisible = newVisibility;

                _repository.UpdateClient(client);
            }
            else
            {
                Console.WriteLine("Error updating client destination");
            }
        }


        public void ClaimClient(Driver driver, string phoneNumber)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);
            client.DriverId = driver.DriverId;
            _repository.UpdateClient(client);
        }

        public Driver? ClientClaimedBy(Client client)
        {
            if (client.ClaimedBy == null) return null;

            return _driverService.GetDriverByPlateNumber(client.ClaimedBy.PlateNumber);
        }

        public Client? GetClaimedClient(Driver driver)
        {
            Client claimedClient = _repository.GetAllClientsWithUsers().First(client => client.ClaimedBy?.PlateNumber == driver.PlateNumber);

            if (claimedClient.User!.IsVisible == true)
            {
                return claimedClient;
            }

            driver.User!.IsVisible = true;
            claimedClient.ClaimedBy = null;
            _driverService.UpdateDriverVisibility(driver);
            _repository.UpdateClient(claimedClient);

            return null;

        }

        public bool IsInTheCar(string phoneNumber)
        {
            Client client = GetClientByPhoneNumber(phoneNumber);
            Driver driver = _driverService.GetDriverByPlateNumber(client.ClaimedBy!.PlateNumber);

            if (driver.User!.Location != null && client.User!.Location != null)
            {
                return CalculateDistance(driver.User.Location, client.User.Location) < 0.003; /// 300 m
            }

            return false;

        }

        public List<Client> GetNearestClients(Driver currentDriver, double currentLongitude, double currentLatitude)
        {
            Location currentLocation = new Location(currentLongitude, currentLatitude);

            List<Client> clients = new List<Client>();


            if (currentDriver.User!.IsVisible == false)
            {
                if (GetClaimedClient(currentDriver) == null)
                {
                    return clients;
                }
                clients.Add(GetClaimedClient(currentDriver)!);

                return clients;
            }

            clients = _repository.GetAllClientsWithUsers();

            List<Client> filteredLocations = clients
            .Where(client =>
                client.User!.IsVisible == true &&
                client.ClaimedBy == null &&
                CalculateDistance(currentLocation, client.User.Location!) <= 60) // Max Distance 60 km
            .OrderBy(client =>
                CalculateDistance(currentLocation, client.User!.Location!))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 clients, otherwise, get all available clients.
            int count = Math.Min(filteredLocations.Count, 10);
            List<Client> nearestLocations = filteredLocations.GetRange(0, count);

            return nearestLocations;
        }

        public static double CalculateDistance(Location location1, Location location2)
        {
            double dLat = DegreesToRadians(location2.Latitude - location1.Latitude);
            double dLon = DegreesToRadians(location2.Longitude - location1.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(DegreesToRadians(location1.Latitude)) * Math.Cos(DegreesToRadians(location2.Latitude)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371 * c; // Distance in kilometers (6371 - Earth radius in kilometers)
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}