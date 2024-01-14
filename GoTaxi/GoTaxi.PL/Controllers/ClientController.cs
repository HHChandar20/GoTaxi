using GoTaxi.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoTaxi.PL.Controllers
{
    // Class representing data structure for destination information
    public class DestinationData
    {
        public string? destination { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public bool visibility { get; set; }
    }

    // Controller responsible for handling client-related actions
    public class ClientController : Controller
    {

        private readonly IClientService _clientService;


        // Constructor that injects the IClientService dependency
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }


        // Action method for rendering the main client view
        public IActionResult Client()
        {
            return View();
        }


        // Action method to update the client's location
        [HttpPost]
        public IActionResult UpdateLocation([FromBody] LocationData locationData)
        {
            try
            {
                if (locationData != null)
                {
                    // Get PhoneNumber from cookie
                    string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
                    _clientService.UpdateClientLocation(phoneNumber, locationData.Longitude, locationData.Latitude);

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


        // Action method to update the client's destination
        [HttpPost]
        public IActionResult UpdateDestination([FromBody] DestinationData destinationData)
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
                _clientService.UpdateClientDestination(phoneNumber, destinationData.destination, destinationData.longitude, destinationData.latitude, destinationData.visibility);

                return Json(new { success = true, message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error updating destination: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }


        // Action method to get the client's destination
        [HttpGet]
        public IActionResult GetClientDestination()
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;

                return Json(_clientService.GetClientByPhoneNumber(phoneNumber).Destination);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        // Action method to get the driver who claimed the client
        [HttpGet]
        public IActionResult ClientClaimedBy()
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;

                return Json(_clientService.ClientClaimedBy(phoneNumber));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(null);
            }
        }


        // Action method to check if the client is visible
        [HttpGet]
        public IActionResult IsClientVisible()
        {
            try
            {
                string phoneNumber = HttpContext.Request.Cookies["CurrentPhoneNumber"]!;

                return Json(_clientService.GetClientByPhoneNumber(phoneNumber).User!.IsVisible);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking client: {ex.Message}");
                return Json(new { success = false, message = "Internal server error" });
            }
        }
    }
}