﻿using GoTaxi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.BLL.Interfaces
{
    public interface IDriverService
    {
        public Driver GetCurrentDriver();
        void AddDriver(string plateNumber, string fullName, string email, string password);
        bool CheckDriver(string plateNumber);
        bool AuthenticateDriver(string plateNumber, string password);
        public void UpdateDriver(string plateNumber, string fullName, string email, string password);
        public void UpdateCurrentDriverLocation(double longitude, double latitude);
        public List<Driver> GetNearestDrivers(double currentDriverLongitude, double currentDriverLatitude);
        Driver ConvertToDriver(string plateNumber, string fullName, string email, string password);
        List<Driver> GetDrivers();
    }
}