using GoTaxi.BLL.Interfaces;
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

                    _driverService.UpdateDriverLocation(plateNumber, locationData.Longitude, locationData.Latitude);
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

                return Json(_driverService.GetNearestDrivers(plateNumber, currentDriverLongitude, currentDriverLatitude));
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

                return Json(_clientService.GetNearestClients(plateNumber, currentClientLongitude, currentClientLatitude));
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

                _driverService.SetDriverVisibility(plateNumber, false);
                _clientService.ClaimClient(plateNumber, phoneNumber);
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

                return Json(_clientService.GetClaimedClient(plateNumber));
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