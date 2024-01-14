using GoTaxi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GoTaxi.DAL.Data
{
    public class GoTaxiDbContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Client> Clients { get; set; }

        public GoTaxiDbContext(DbContextOptions<GoTaxiDbContext> options) : base(options)
        {
        }
    }
}