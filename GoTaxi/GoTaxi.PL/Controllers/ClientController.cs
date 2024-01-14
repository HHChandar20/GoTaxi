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

        // Get current user details from cookies
        private string CurrentPhoneNumber => HttpContext.Request.Cookies["CurrentPhoneNumber"]!;
        private string? CurrentUserType => HttpContext.Request.Cookies["CurrentUserType"];


        // Constructor that injects the IClientService dependency
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }


        // Action method for rendering the main client view
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Client()
        {

            if (CurrentUserType == "Driver" || CurrentUserType == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        // Action method to update the client's location
        [HttpPost]
        public IActionResult UpdateLocation([FromBody] LocationData locationData)
        {
            try
            {
                _clientService.UpdateClientLocation(CurrentPhoneNumber, locationData.Longitude, locationData.Latitude);

                return Json(new { success = true, message = "Location updated successfully" });
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
                _clientService.UpdateClientDestination(CurrentPhoneNumber, destinationData.destination, destinationData.longitude, destinationData.latitude, destinationData.visibility);

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
                return Json(_clientService.GetClientByPhoneNumber(CurrentPhoneNumber).Destination);
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
                return Json(_clientService.ClientClaimedBy(CurrentPhoneNumber));
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
                return Json(_clientService.GetClientByPhoneNumber(CurrentPhoneNumber).User!.IsVisible);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking client: {ex.Message}");
                return Json(false);
            }
        }
    }
}