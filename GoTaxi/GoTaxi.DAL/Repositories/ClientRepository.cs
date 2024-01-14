using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;

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

        public List<Client> GetAllClientsExceptCurrent(string phoneNumber)
        {
            return _context.Clients.Where(client => client.PhoneNumber != phoneNumber).ToList();
        }

        public Client GetClientByPlateNumber(string phoneNumber)
        {
            return _context.Clients.FirstOrDefault(client => client.PhoneNumber == phoneNumber);
        }

        public void AddClient(Client newClient)
        {
            _context.Clients.Add(newClient);
            _context.SaveChanges();
        }

        public void UpdateClient(Client updatedClient)
        {
            var existingClient = _context.Clients.FirstOrDefault(client => client.PhoneNumber == updatedClient.PhoneNumber);

            if (existingClient != null)
            {
                existingClient.Email = updatedClient.Email;
                existingClient.FullName = updatedClient.FullName;
                existingClient.Password = updatedClient.Password;
                existingClient.Longitude = updatedClient.Longitude;
                existingClient.Latitude = updatedClient.Latitude;

                _context.SaveChanges();
            }
        }
    }
}