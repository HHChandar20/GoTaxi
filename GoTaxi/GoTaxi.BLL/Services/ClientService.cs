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
            return _repository.GetAllClients()?.Any(client => client.PhoneNumber == phoneNumber) ?? false;
        }

        public bool AuthenticateClient(string phoneNumber, string password)
        {
            return _repository.GetAllClientsWithUsers()
                .Any(client => client.PhoneNumber == phoneNumber && client.User?.Password == password);
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

        public void AddClient(Client client)
        {
            _repository.AddClient(client);
        }

        public void UpdateClient(string phoneNumber, string fullName, string email, string password)
        {
            _repository.UpdateClient(ConvertToClient(phoneNumber, fullName, email, password));
        }

        public void UpdateClientLocation(string phoneNumber, double longitude, double latitude)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);

            if (client != null && client.User != null && client.User.Location != null)
            {
                client.User.Location.Longitude = longitude;
                client.User.Location.Latitude = latitude;

                _repository.UpdateClient(client);
            }
        }

        public void UpdateClientDestination(string phoneNumber, string? destination, double longitude, double latitude, bool visibility)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);

            if (client != null && client.Destination != null)
            {
                Driver driver;

                if (client.ClaimedBy != null)
                {
                    driver = _driverService.GetDriverByPlateNumber(client.ClaimedBy.PlateNumber);
                    driver.User!.IsVisible = !visibility;

                    // Client canceled the request
                    if (!visibility)
                    {
                        _driverService.UpdateDriverVisibility(driver);
                        client.ClaimedBy = null;
                    }
                }

                client.Destination.Name = destination;
                client.Destination.Location!.Longitude = longitude;
                client.Destination.Location.Latitude = latitude;
                client.User!.IsVisible = visibility;

                _repository.UpdateClient(client);
            }
        }

        public void ClaimClient(string plateNumber, string phoneNumber)
        {
            Driver driver = _driverService.GetDriverByPlateNumber(plateNumber);
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);

            client.DriverId = driver.DriverId;

            _repository.UpdateClient(client);
        }

        public Driver? ClientClaimedBy(string phoneNumber)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);

            if (client.ClaimedBy == null)
            {
                return null;
            }

            return _driverService.GetDriverByPlateNumber(client.ClaimedBy.PlateNumber);
        }

        public Client? GetClaimedClient(string plateNumber)
        {
            Driver driver = _driverService.GetDriverByPlateNumber(plateNumber);
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
                return DistanceCalculator.CalculateDistance(driver.User.Location, client.User.Location) < DistanceCalculator.CarRange; // Distance 33 m
            }

            return false;

        }

        public List<Client> GetNearestClients(string plateNumber, double longitude, double latitude)
        {
            Location currentLocation = new Location(longitude, latitude);

            Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
            List<Client> clients = new List<Client>();


            if (currentDriver.User!.IsVisible == false)
            {
                clients.Add(GetClaimedClient(plateNumber)!);

                return clients;
            }

            clients = _repository.GetAllClientsWithUsers();

            List<Client> filteredClients = clients
            .Where(client =>
                client.User!.IsVisible == true &&
                client.ClaimedBy == null &&
                DistanceCalculator.CalculateDistance(currentLocation, client.User.Location!) <= DistanceCalculator.Range) // Max Distance 60 km
            .OrderBy(client =>
                DistanceCalculator.CalculateDistance(currentLocation, client.User!.Location!))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 clients, otherwise, get all available clients.
            int count = Math.Min(filteredClients.Count, 10);
            List<Client> nearestLocations = filteredClients.GetRange(0, count);

            return nearestLocations;
        }
    }
}