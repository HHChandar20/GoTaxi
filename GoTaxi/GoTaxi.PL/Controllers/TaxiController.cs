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
                    _driverService.UpdateCurrentDriverLocation(locationData.Longitude, locationData.Latitude);
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
                List<Driver> nearestDrivers = _driverService.GetNearestDrivers(currentDriverLongitude, currentDriverLatitude);
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
                List<Client> nearestClients = _clientService.GetNearestClients(currentClientLongitude, currentClientLatitude);
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
                _driverService.SetCurrentDriverVisibility(true);
                _clientService.ClaimClient(phoneNumber);
                return Json(new { success = true, message = "Location updated successfully" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error claiming client 2: {ex.Message} Phone: {phoneNumber}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }

        
        [HttpGet]
        public IActionResult CheckClaimedClient()
        {
            try
            {
                Client claimedClient = _clientService.GetClaimedClient();
                return Json(claimedClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching claimed client: {ex.Message}");
                return Json(new { success = false, message = "Error fetching claimed client" });
            }
        }

    }

}