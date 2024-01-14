using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Interfaces
{
    public interface IDriverService
    {
        void AddDriver(string plateNumber, string fullName, string email, string password);
        bool CheckDriver(string plateNumber);
        Driver ConvertToDriver(string plateNumber, string fullName, string email, string password);
        List<Driver> GetDrivers();
    }
}