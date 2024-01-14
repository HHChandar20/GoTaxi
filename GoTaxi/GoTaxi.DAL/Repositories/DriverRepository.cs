using GoTaxi.DAL.Models;

namespace GoTaxi.DAL.Repositories
{
    public class DriverRepository
    {
        private static DriverRepository instance = null;
        private List<Driver> driversList = new List<Driver> { new Driver() };

        public static DriverRepository GetInstance()
        {
            if (instance == null)
            {
                instance = new DriverRepository();
            }

            return instance;
        }

        private DriverRepository()
        {

        }

        // Read multiple
        public List<Driver> GetDrivers()
        {
            return driversList;
        }

        // Read single
        public Driver GetDriverByPlateNumber(string plateNumber)
        {
            return driversList.Find(driver => driver.PlateNumber == plateNumber);
        }

        // Create
        public void AddDriver(Driver newDriver)
        {
            driversList.Add(newDriver);
        }

    }
}