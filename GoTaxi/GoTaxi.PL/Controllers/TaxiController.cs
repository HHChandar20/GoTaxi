using GoTaxi.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoTaxi.PL.Controllers
{
    // Class representing data structure for location information
    public class LocationData
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    // Controller responsible for handling driver-related actions
    public class TaxiController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IClientService _clientService;

        // Get current user details from cookies
        private string CurrentPlateNumber => HttpContext.Request.Cookies["CurrentPlateNumber"]!;
        private string? CurrentUserType => HttpContext.Request.Cookies["CurrentUserType"];


        // Constructor to inject dependencies for driver and client services
        public TaxiController(IDriverService driverService, IClientService clientService)
        {
            _driverService = driverService;
            _clientService = clientService;
        }

        // Action method to display the taxi-related view
        public IActionResult Taxi()
        {
            if (CurrentUserType == "Client" || CurrentUserType == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Action method to update the current driver's location
        [HttpPost]
        public IActionResult UpdateLocation([FromBody] LocationData locationData)
        {
            try
            {
                _driverService.UpdateDriverLocation(CurrentPlateNumber, locationData.Longitude, locationData.Latitude);

                return Json(new { success = true, message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating location: {ex.Message}");
                return Json(null);
            }
        }


        // Action method to get the nearest drivers based on the current driver's location
        [HttpGet]
        public IActionResult GetNearestDrivers(double currentDriverLongitude, double currentDriverLatitude)
        {
            try
            {
                return Json(_driverService.GetNearestDrivers(CurrentPlateNumber, currentDriverLongitude, currentDriverLatitude));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching nearest drivers: {ex.Message}");
                return Json(null);
            }
        }


        // Action method to get the nearest clients based on the current driver's location
        [HttpGet]
        public IActionResult GetNearestClients(double currentClientLongitude, double currentClientLatitude)
        {
            try
            {
                return Json(_clientService.GetNearestClients(CurrentPlateNumber, currentClientLongitude, currentClientLatitude));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching nearest clients: {ex.Message}");
                return Json(null);
            }
        }


        // Action method to claim a client by a driver
        [HttpPost]
        public IActionResult ClaimClient([FromBody] string phoneNumber)
        {
            try
            {
                _driverService.SetDriverVisibility(CurrentPlateNumber, false);
                _clientService.ClaimClient(CurrentPlateNumber, phoneNumber);
                return Json(new { success = true, message = "Location updated successfully" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error claiming client: {ex.Message} Phone: {phoneNumber}");
                return Json(null);
            }
        }


        // Action method to check if a client is claimed by the current driver
        [HttpGet]
        public IActionResult CheckClaimedClient()
        {
            try
            {
                return Json(_clientService.GetClaimedClient(CurrentPlateNumber));
            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        // Action method to check if a client is currently in the car
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