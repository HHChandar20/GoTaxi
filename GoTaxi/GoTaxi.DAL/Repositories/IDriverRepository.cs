using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Repositories
{
    public interface IDriverRepository
    {
        List<Driver> GetAllDrivers();
        List<Driver> GetAllDriversWithUsers();
        List<Driver> GetAllDriversExceptCurrentWithUsers(string plateNumber);
        Driver GetDriverByPlateNumber(string plateNumber);
        void AddDriver(Driver newDriver);
        void UpdateDriver(Driver updatedDriver);
    }
}