using GoTaxi.DAL.Models;
using GoTaxi.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Interfaces
{
    public interface IClientService
    {
        Client GetClientByPhoneNumber(string phoneNumber);
        void AddClient(string phoneNumber, string fullName, string email, string password);
        void AddClient(Client client);
        bool CheckClient(string phoneNumber);
        bool AuthenticateClient(string phoneNumber, string password);
        void UpdateClient(string phoneNumber, string fullName, string email, string password);
        void UpdateClientLocation(string phoneNumber, double longitude, double latitude);
        void UpdateClientDestination(string phoneNumber, string? newDestination, double newLongitude, double newLatitude, bool newVisibility);
        void ClaimClient(string plateNumber, string phoneNumber);
        bool IsInTheCar(string phoneNumber);
        Client? GetClaimedClient(string plateNumber);
        Driver? ClientClaimedBy(string phoneNumber);
        List<Client> GetNearestClients(string plateNumber, double currentClientLongitude, double currentClientLatitude);
        Client ConvertToClient(string phoneNumber, string fullName, string email, string password);
        List<Client> GetClients();

    }
}