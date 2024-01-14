using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GoTaxi.DAL.Data
{
    // DbContext class responsible for interacting with the database
    public class GoTaxiDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Client> Clients { get; set; }

        public GoTaxiDbContext(DbContextOptions<GoTaxiDbContext> options) : base(options)
        {
            // Empty constructor body as it just calls the base class constructor
        }
    }
}