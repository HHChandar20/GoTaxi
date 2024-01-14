using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Interfaces
{
    // Interface for defining operations related to drivers in the application
    public interface IDriverService
    {
        void SetDriverVisibility(string plateNumber, bool visibility);

        Driver GetDriverByPlateNumber(string plateNumber);

        void AddDriver(string plateNumber, string fullName, string email, string password);

        void AddDriver(Driver driver);

        // Check if a driver with the given plate number exists.
        bool CheckDriver(string plateNumber);

        bool AuthenticateDriver(string plateNumber, string password);

        void UpdateDriver(string plateNumber, string fullName, string email, string password);

        void UpdateDriver(Driver driver);

        void UpdateDriverVisibility(string plateNumber);

        void UpdateDriverVisibility(Driver driver);

        void UpdateDriverLocation(string plateNumber, double longitude, double latitude);

        List<Driver> GetNearestDrivers(string plateNumber, double longitude, double latitude);

        Driver ConvertToDriver(string plateNumber, string fullName, string email, string password);

        List<Driver> GetDrivers();
    }
}