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
        public Client GetClientByPhoneNumber(string phoneNumber);
        public void AddClient(string phoneNumber, string fullName, string email, string password);
        public bool CheckClient(string phoneNumber);
        public Client? AuthenticateClient(string phoneNumber, string password);
        public void UpdateClient(string phoneNumber, string fullName, string email, string password);
        public void UpdateClientLocation(Client client, double longitude, double latitude);
        public void UpdateClientDestination(Client client, string? newDestination, double newLongitude, double newLatitude, bool newVisibility);
        public void ClaimClient(Driver driver, string phoneNumber);
        public bool IsInTheCar(string phoneNumber);
        public Client GetClaimedClient(Driver driver);
        public Driver? ClientClaimedBy(Client client);
        public List<Client> GetNearestClients(Driver driver, double currentClientLongitude, double currentClientLatitude);
        Client ConvertToClient(string phoneNumber, string fullName, string email, string password);
        List<Client> GetClients();

    }
}