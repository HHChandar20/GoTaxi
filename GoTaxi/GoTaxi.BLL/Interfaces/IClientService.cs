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
        void AddClient(string phoneNumber, string fullName, string email, string password);
        bool CheckClient(string phoneNumber);
        bool AuthenticateClient(string phoneNumber, string password);
        public void UpdateClient(string phoneNumber, string fullName, string email, string password);
        public void UpdateCurrentClientLocation(double longitude, double latitude);
        public void UpdateCurrentClientDestination(string newDestination, bool newVisibility);
        public void ClaimClient(string phoneNumber);
        public Driver ClientClaimedBy();
        public List<Client> GetNearestClients(double currentClientLongitude, double currentClientLatitude);
        Client ConvertToClient(string phoneNumber, string fullName, string email, string password);
        List<Client> GetClients();

    }
}