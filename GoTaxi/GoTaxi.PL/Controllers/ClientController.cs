using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoTaxi.PL.Controllers
{
    public class DestinationData
    {
        public string? NewDestination { get; set; }
        public double NewLongitude { get; set; }
        public double NewLatitude { get; set; }
        public bool NewVisibility { get; set; }
    }

    public class ClientController : Controller
    {

        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public IActionResult Client()
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
                    // Get PhoneNumber from cookie
                    string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;

                    Client currentClient = _clientService.GetClientByPhoneNumber(phoneNumber);
                    _clientService.UpdateClientLocation(currentClient, locationData.Longitude, locationData.Latitude);
                    return Json(new { success = true, message = "Location updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid location data" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error updating location: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost]
        public IActionResult UpdateDestination([FromBody] DestinationData destinationData)
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
                Client currentClient = _clientService.GetClientByPhoneNumber(phoneNumber);
                _clientService.UpdateClientDestination(currentClient, destinationData.NewDestination, destinationData.NewLongitude, destinationData.NewLatitude, destinationData.NewVisibility);

                return Json(new { success = true, message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error updating destination: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }


        [HttpGet]
        public IActionResult GetClientDestination()
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
                Client currentClient = _clientService.GetClientByPhoneNumber(phoneNumber);
                return Json(currentClient.Destination);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking client: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet]
        public IActionResult ClientClaimedBy()
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
                Client currentClient = _clientService.GetClientByPhoneNumber(phoneNumber);

                return Json(_clientService.ClientClaimedBy(currentClient));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(null);
            }
        }

        [HttpGet]
        public IActionResult IsClientVisible()
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
                Client currentClient = _clientService.GetClientByPhoneNumber(phoneNumber);
                return Json(currentClient.User!.IsVisible);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking client: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }
    }
}