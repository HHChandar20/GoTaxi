using GoTaxi.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoTaxi.PL.Controllers
{
    public class DestinationData
    {
        public string? destination { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public bool visibility { get; set; }
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