using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GoTaxi.DAL.Repositories
{
    // Repository class for handling CRUD operations related to Client entities
    public class ClientRepository
    {
        // The DbContext used for interacting with the database
        private readonly GoTaxiDbContext _context;

        // Constructor that initializes the repository with a DbContext
        public ClientRepository(GoTaxiDbContext context)
        {
            _context = context;
        }

        // Retrieve all clients from the database
        public List<Client> GetAllClients()
        {
            return _context.Clients.ToList();
        }

        // Retrieve all clients with associated users, destinations, and claimed by information
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

        // Retrieve all clients with associated destinations
        public List<Client> GetAllClientsWithDestinations()
        {
            return _context.Clients.Include(c => c.Destination).ToList();
        }

        // Retrieve all clients with associated claimed by information
        public List<Client> GetAllClientsWithClaimedBy()
        {
            return _context.Clients.Include(c => c.ClaimedBy).ToList();
        }

        // Retrieve all clients except the one with the provided phone number
        public List<Client> GetAllClientsExceptCurrent(string phoneNumber)
        {
            return _context.Clients.Where(client => client.PhoneNumber != phoneNumber).ToList();
        }

        // Retrieve a client by phone number with associated user, destination, and claimed by information
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

        // Add a new client to the database
        public void AddClient(Client newClient)
        {
            _context.Clients.Add(newClient);
            _context.SaveChanges();
        }

        // Update an existing client in the database
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