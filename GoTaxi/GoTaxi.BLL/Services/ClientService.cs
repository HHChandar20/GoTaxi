using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace GoTaxi.BLL.Services
{
    public class ClientService : IClientService
    {
        public static Client currentClient = new Client();
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
            List<Client> clients = _repository.GetAllClients();

            if (clients == null)
            {
                return false;
            }

            foreach (Client client in clients)
            {
                if (client.PhoneNumber == phoneNumber && client.Password == password)
                {
                    currentClient = client;
                    return true;
                }
            }

            return false;
        }

        public Client ConvertToClient(string phoneNumber, string fullName, string email, string password)
        {
            Client client = new Client();

            client.PhoneNumber = phoneNumber;
            client.Email = email;
            client.FullName = fullName;
            client.Password = password;

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

        public void UpdateCurrentClientLocation(double longitude, double latitude)
        {
            currentClient.Longitude = longitude;
            currentClient.Latitude = latitude;

            _repository.UpdateClient(currentClient);
        }

        public void UpdateCurrentClientDestination(string newDestination)
        {
            currentClient.Destination = newDestination;

            _repository.UpdateClient(currentClient);
        }

        public void ClaimClient(string phoneNumber)
        {
            Console.WriteLine(phoneNumber.IsNullOrEmpty().ToString());
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);
            client.ClaimedBy = _driverService.GetCurrentDriver().PlateNumber;
            _repository.UpdateClient(client);
        }

        public List<Client> GetNearestClients(double currentClientLongitude, double currentClientLatitude)
        {
            List<Client> clients = _repository.GetAllClients();

            if (clients == null)
            {
                return new List<Client>(); // Return an empty list if there are no other clients or only the current client.
            }

            clients.Remove(currentClient);

            List<Client> filteredLocations = clients
            .Where(client =>
                CalculateDistance(currentClientLongitude, currentClientLatitude, client.Longitude, client.Latitude) <= 60)
            .OrderBy(client =>
                CalculateDistance(currentClientLongitude, currentClientLatitude, client.Longitude, client.Latitude))
            .ToList();

            // Get the nearest 10 locations if there are at least 10 clients, otherwise, get all available clients.
            int count = Math.Min(filteredLocations.Count, 10);
            List<Client> nearestLocations = filteredLocations.GetRange(0, count);

            return nearestLocations;
        }

        public static double CalculateDistance(double longitude1, double latitude1, double longitude2, double latitude2)
        {
            double dLat = DegreesToRadians(latitude2 - latitude1);
            double dLon = DegreesToRadians(longitude2 - longitude1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(DegreesToRadians(latitude1)) * Math.Cos(DegreesToRadians(latitude2)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371 * c; // Distance in kilometers // 6371 - Earth radius in kilometers
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}