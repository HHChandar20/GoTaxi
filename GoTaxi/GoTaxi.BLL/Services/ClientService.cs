using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;

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

        public Client GetCurrentClient()
        {
            return currentClient;
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

        public void UpdateCurrentClientDestination(string newDestination, bool newVisibility)
        {
            currentClient.Destination = newDestination;
            currentClient.IsVisible = newVisibility;

            Driver driver = new Driver();

            if (currentClient.ClaimedBy != null)
            {
                driver = _driverService.GetDriverByPlateNumber(currentClient.ClaimedBy);
            }

            if (!newVisibility && driver != null)
            {
                driver.IsVisible = true;
                _driverService.UpdateDriver(driver);

                currentClient.ClaimedBy = null;
            }

            _repository.UpdateClient(currentClient);
        }

        public void ClaimClient(string phoneNumber)
        {
            Client client = _repository.GetClientByPhoneNumber(phoneNumber);
            client.ClaimedBy = _driverService.GetCurrentDriver().PlateNumber;
            _repository.UpdateClient(client);
        }

        public Driver ClientClaimedBy()
        {
            currentClient = _repository.GetClientByPhoneNumber(currentClient.PhoneNumber);

            if (currentClient.ClaimedBy == null) return null;

            return _driverService.GetDriverByPlateNumber(currentClient.ClaimedBy);
        }

        public Client GetClaimedClient()
        {
            return _repository.GetAllClients().First(client => client.ClaimedBy == DriverService.currentDriver.PlateNumber);
        }

        public bool IsInTheCar()
        {
            Driver driver = DriverService.currentDriver;
            Client client = GetClaimedClient();

            return CalculateDistance(driver.Longitude, driver.Latitude, client.Longitude, client.Latitude) < 0.002;
        }

        public double[] GetClaimedClientLocation()
        {
            Client claimedClient = _repository.GetAllClients().First(client => client.ClaimedBy == DriverService.currentDriver.PlateNumber);
            return new double[] { claimedClient.Longitude, claimedClient.Latitude };
        }


        public List<Client> GetNearestClients(double currentClientLongitude, double currentClientLatitude)
        {
            List<Client> clients = _repository.GetAllClients();

            if (clients == null)
            {
                return new List<Client>(); // Return an empty list if there are no other clients or only the current client.
            }

            //clients.Remove(currentClient);

            List<Client> filteredLocations = clients
            .Where(client =>
                client.IsVisible == true &&
                (client.ClaimedBy == null || client.ClaimedBy == DriverService.currentDriver.PlateNumber) &&
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