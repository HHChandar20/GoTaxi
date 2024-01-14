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

        public bool AuthenticateClient(string phoneNumber, string password)
        {
            Client? client = _repository.GetAllClientsWithUsers().FirstOrDefault(client => client.PhoneNumber == phoneNumber && client.User?.Password == password);

            if (client == null)
            {
                return false;
            }

            return true;
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

        public void UpdateClientLocation(string phoneNumber, double newLongitude, double newLatitude)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);

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

        public void UpdateClientDestination(string phoneNumber, string? newDestination, double newLongitude, double newLatitude, bool newVisibility)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);

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
                return DistanceCalculator.CalculateDistance(driver.User.Location, client.User.Location) < 0.033; /// 33 m
            }

            return false;

        }

        public List<Client> GetNearestClients(string plateNumber, double currentLongitude, double currentLatitude)
        {
            Location currentLocation = new Location(currentLongitude, currentLatitude);

            Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
            List<Client> clients = new List<Client>();


            if (currentDriver.User!.IsVisible == false)
            {
                if (GetClaimedClient(plateNumber) == null)
                {
                    return clients;
                }
                clients.Add(GetClaimedClient(plateNumber)!);

                return clients;
            }

            clients = _repository.GetAllClientsWithUsers();

            List<Client> filteredLocations = clients
            .Where(client =>
                client.User!.IsVisible == true &&
                client.ClaimedBy == null &&
                DistanceCalculator.CalculateDistance(currentLocation, client.User.Location!) <= 60) // Max Distance 60 km
            .OrderBy(client =>
                DistanceCalculator.CalculateDistance(currentLocation, client.User!.Location!))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 clients, otherwise, get all available clients.
            int count = Math.Min(filteredLocations.Count, 10);
            List<Client> nearestLocations = filteredLocations.GetRange(0, count);

            return nearestLocations;
        }
    }
}