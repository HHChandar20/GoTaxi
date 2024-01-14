using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoTaxi.PL.Controllers
{
    public class LocationData
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public class TaxiController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IClientService _clientService;

        public TaxiController(IDriverService driverService, IClientService clientService)
        {
            _driverService = driverService;
            _clientService = clientService;
        }

        public IActionResult Taxi()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateLocation([FromBody] LocationData locationData)
        {
            try
            {
                if (locationData != null)
                {
                    // Get PlateNumber from cookie
                    string plateNumber = HttpContext.Request.Cookies["CurrentPlateNumber"]!;

                    Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
                    _driverService.UpdateDriverLocation(currentDriver, locationData.Longitude, locationData.Latitude);
                    return Json(new { success = true, message = "Location updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid location data" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating location: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet]
        public IActionResult GetNearestDrivers(double currentDriverLongitude, double currentDriverLatitude)
        {
            try
            {
                string plateNumber = HttpContext.Request.Cookies["CurrentPlateNumber"]!;
                Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
                List<Driver> nearestDrivers = _driverService.GetNearestDrivers(currentDriver, currentDriverLongitude, currentDriverLatitude);
                return Json(nearestDrivers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching nearest drivers: {ex.Message}");
                return Json(new { success = false, message = "Error fetching nearest drivers" });
            }
        }

        [HttpGet]
        public IActionResult GetNearestClients(double currentClientLongitude, double currentClientLatitude)
        {
            try
            {
                string plateNumber = HttpContext.Request.Cookies["CurrentPlateNumber"]!;
                Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
                List<Client> nearestClients = _clientService.GetNearestClients(currentDriver, currentClientLongitude, currentClientLatitude);

                return Json(nearestClients);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching nearest clients: {ex.Message}");
                return Json(new { success = false, message = "Error fetching nearest clients" });
            }
        }


        [HttpPost]
        public IActionResult ClaimClient([FromBody] string phoneNumber)
        {
            try
            {
                string plateNumber = HttpContext.Request.Cookies["CurrentPlateNumber"]!;
                Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
                _driverService.SetDriverVisibility(currentDriver, false);
                _clientService.ClaimClient(currentDriver, phoneNumber);
                return Json(new { success = true, message = "Location updated successfully" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error claiming client: {ex.Message} Phone: {phoneNumber}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }


        [HttpGet]
        public IActionResult CheckClaimedClient()
        {
            try
            {
                string plateNumber = HttpContext.Request.Cookies["CurrentPlateNumber"]!;
                Driver currentDriver = _driverService.GetDriverByPlateNumber(plateNumber);
                Client? claimedClient = _clientService.GetClaimedClient(currentDriver);

                return Json(claimedClient);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        [HttpGet]
        public IActionResult IsInTheCar(string phoneNumber)
        {
            try
            {
                return Json(_clientService.IsInTheCar(phoneNumber));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if the client is in the car: {ex.Message}");
                return Json(false);
            }
        }

    }

}