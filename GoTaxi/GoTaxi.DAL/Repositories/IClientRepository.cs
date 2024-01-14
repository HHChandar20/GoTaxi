using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Repositories
{
    public interface IClientRepository
    {
        List<Client> GetAllClients();
        List<Client> GetAllClientsWithUsers();
        List<Client> GetAllClientsWithDestinations();
        List<Client> GetAllClientsWithClaimedBy();
        List<Client> GetAllClientsExceptCurrent(string phoneNumber);
        Client GetClientByPhoneNumber(string phoneNumber);
        void AddClient(Client newClient);
        void UpdateClient(Client updatedClient);
    }
}