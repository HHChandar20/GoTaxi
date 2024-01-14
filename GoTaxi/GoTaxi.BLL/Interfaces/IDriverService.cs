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
        public void SetDriverVisibility(Driver currentDriver, bool visibility);
        public Driver GetDriverByPlateNumber(string plateNumber);
        void AddDriver(string plateNumber, string fullName, string email, string password);
        void AddDriver(Driver driver);
        bool CheckDriver(string plateNumber);
        Driver? AuthenticateDriver(string plateNumber, string password);
        public void UpdateDriver(string plateNumber, string fullName, string email, string password);
        public void UpdateDriver(Driver driver);
        public void UpdateDriverVisibility(Driver driver);
        public void UpdateDriverLocation(Driver driver, double longitude, double latitude);
        public List<Driver> GetNearestDrivers(Driver currentDriver, double currentDriverLongitude, double currentDriverLatitude);
        Driver ConvertToDriver(string plateNumber, string fullName, string email, string password);
        List<Driver> GetDrivers();
    }
}