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
            return _repository.GetAllClients().First(client => client.PhoneNumber == phoneNumber && client.Password == password);
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

        public void UpdateClientLocation(Client client, double longitude, double latitude)
        {

            client.Longitude = longitude;
            client.Latitude = latitude;

            _repository.UpdateClient(client);
        }

        public void UpdateClientDestination(Client client, string? newDestination, double newLongitude, double newLatitude, bool newVisibility)
        {
            // Additional logic to handle driver visibility
            Driver driver;

            if (client.ClaimedBy != null)
            {
                driver = _driverService.GetDriverByPlateNumber(client.ClaimedBy);
                driver.IsVisible = !newVisibility;

                if (!newVisibility)
                {
                    // Client canceled the request
                    _driverService.UpdateDriverVisibility(driver);
                    client.ClaimedBy = null;
                }
            }

            client.Destination = newDestination;
            client.DestinationLongitude = newLongitude;
            client.DestinationLatitude = newLatitude;
            client.IsVisible = newVisibility;

            _repository.UpdateClient(client);
        }


        public void ClaimClient(Driver driver, string phoneNumber)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);
            client.ClaimedBy = driver.PlateNumber;
            _repository.UpdateClient(client);
        }

        public Driver? ClientClaimedBy(Client client)
        {
            if (client.ClaimedBy == null) return null;

            return _driverService.GetDriverByPlateNumber(client.ClaimedBy);
        }

        public Client GetClaimedClient(Driver driver)
        {
            return _repository.GetAllClients().First(client => client.ClaimedBy == driver.PlateNumber);
        }

        public bool IsInTheCar(Driver driver, Client client)
        {
            return CalculateDistance(driver.Longitude, driver.Latitude, client.Longitude, client.Latitude) < 0.3; /// 300 m
        }

        public List<Client> GetNearestClients(Driver driver, double currentClientLongitude, double currentClientLatitude)
        {
            List<Client> clients = _repository.GetAllClients();

            if (clients == null)
            {
                return new List<Client>();
            }

            //clients.Remove(currentClient);

            List<Client> filteredLocations = clients
            .Where(client =>
                client.IsVisible == true &&
                (client.ClaimedBy == null || client.ClaimedBy == driver.PlateNumber) &&
                CalculateDistance(currentClientLongitude, currentClientLatitude, client.Longitude, client.Latitude) <= 60) // Max Distance 60 km
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