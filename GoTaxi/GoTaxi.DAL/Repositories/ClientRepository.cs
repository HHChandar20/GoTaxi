using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GoTaxi.DAL.Repositories
{
    public class ClientRepository
    {
        private readonly GoTaxiDbContext _context;

        public ClientRepository(GoTaxiDbContext context)
        {
            _context = context;
        }

        public List<Client> GetAllClients()
        {
            return _context.Clients.ToList();
        }
        public List<Client> GetAllClientsWithUsers()
        {
            return _context.Clients
                .Include(c => c.User)
                    .ThenInclude(u => u!.Location)
                .Include(c => c.Destination)
                    .ThenInclude(d => d!.Location)
                .Include(c => c.ClaimedBy)
                .ToList();
        }

        public List<Client> GetAllClientsWithDestinations()
        {
            return _context.Clients.Include(c => c.Destination).ToList();
        }

        public List<Client> GetAllClientsWithClaimedBy()
        {
            return _context.Clients.Include(c => c.ClaimedBy).ToList();
        }

        public List<Client> GetAllClientsExceptCurrent(string phoneNumber)
        {
            return _context.Clients.Where(client => client.PhoneNumber != phoneNumber).ToList();
        }


        public Client GetClientByPhoneNumber(string phoneNumber)
        {
            return _context.Clients
                .Include(c => c.User)
                    .ThenInclude(u => u!.Location)
                .Include(c => c.Destination)
                    .ThenInclude(d => d!.Location)
                .Include(c => c.ClaimedBy)
                .First(client => client.PhoneNumber == phoneNumber);
        }

        public void AddClient(Client newClient)
        {
            _context.Clients.Add(newClient);
            _context.SaveChanges();
        }

        public void UpdateClient(Client updatedClient)
        {
            Client existingClient = GetClientByPhoneNumber(updatedClient.PhoneNumber);

            if (existingClient != null)
            {
                _context.Entry(existingClient).CurrentValues.SetValues(updatedClient);
                _context.SaveChanges();
            }
        }
    }
}