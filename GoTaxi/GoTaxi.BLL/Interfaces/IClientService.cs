using GoTaxi.DAL.Models;

namespace GoTaxi.BLL.Interfaces
{
    // Interface for defining operations related to clients in the application
    public interface IClientService
    {
        Client GetClientByPhoneNumber(string phoneNumber);

        void AddClient(string phoneNumber, string fullName, string email, string password);

        void AddClient(Client client);

        // Check if a client with the given phone number exists
        bool CheckClient(string phoneNumber);

        // Authenticate a client using phone number and password
        bool AuthenticateClient(string phoneNumber, string password);

        void UpdateClient(string phoneNumber, string fullName, string email, string password);

        void UpdateClientLocation(string phoneNumber, double longitude, double latitude);

        void UpdateClientDestination(string phoneNumber, string? destination, double longitude, double latitude, bool visibility);

        void ClaimClient(string plateNumber, string phoneNumber);

        // Check if a client is currently in the car
        bool IsInTheCar(string phoneNumber);

        Client? GetClaimedClient(string plateNumber);

        // Get the driver who claimed a specific client using their phone number
        Driver? ClientClaimedBy(string phoneNumber);

        // Get a list of nearest clients based on a driver's location
        List<Client> GetNearestClients(string plateNumber, double longitude, double latitude);

        // Convert information into a Client object
        Client ConvertToClient(string phoneNumber, string fullName, string email, string password);

        List<Client> GetClients();

    }
}