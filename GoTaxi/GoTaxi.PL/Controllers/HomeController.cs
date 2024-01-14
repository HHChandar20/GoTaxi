using Microsoft.AspNetCore.Mvc;
using GoTaxi.BLL.Interfaces;

namespace GoTaxi.PL.Controllers
{
    // Controller responsible for handling login and registration actions
    public class HomeController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IClientService _clientService;

        // Constructor to inject dependencies for driver and client services
        public HomeController(IDriverService driverService, IClientService clientService)
        {
            _driverService = driverService;
            _clientService = clientService;
        }

        // Action method to display the home page
        public IActionResult Index()
        {
            return View();
        }

        // Action method to display the login page
        public IActionResult Login()
        {

            return View();
        }

        // Action method to display the registration page
        public IActionResult Register()
        {
            return View();
        }


        // Action method to handle driver login
        [HttpPost]
        public IActionResult LoginDriver(string plateNumber, string password)
        {
            if (!_driverService.CheckDriver(plateNumber))
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToAction("Login");
            }

            if (!_driverService.AuthenticateDriver(plateNumber, password))
            {
                TempData["ErrorMessage"] = "Incorrect credentials.";
                return RedirectToAction("Login");
            }

            // Store current driver in cookies
            HttpContext.Response.Cookies.Append("CurrentUserType", "Driver");
            HttpContext.Response.Cookies.Append("CurrentPlateNumber", plateNumber);

            return RedirectToAction("Taxi", "Taxi");
        }

        // Action method to handle client login
        [HttpPost]
        public IActionResult LoginClient(string phoneNumber, string password)
        {
            if (!_clientService.CheckClient(phoneNumber))
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToAction("Login");
            }

            if (!_clientService.AuthenticateClient(phoneNumber, password))
            {
                TempData["ErrorMessage"] = "Incorrect credentials.";
                return RedirectToAction("Login");
            }

            // Store current driver in cookies
            HttpContext.Response.Cookies.Append("CurrentUserType", "Client");
            HttpContext.Response.Cookies.Append("CurrentPhoneNumber", phoneNumber);

            return RedirectToAction("Client", "Client");
        }

        // Action method to handle driver registration
        [HttpPost]
        public IActionResult RegisterDriver(string plateNumber, string email, string fullName, string password)
        {
            if (!_driverService.CheckDriver(plateNumber))
            {
                _driverService.AddDriver(plateNumber, email, fullName, password);
                TempData["SuccessMessage"] = "Account registered successfully. You can now log in.";
                return RedirectToAction("Register");
            }

            TempData["ErrorMessage"] = "Account with this plate number already exists.";
            return RedirectToAction("Register");
        }

        // Action method to handle client registration
        [HttpPost]
        public IActionResult RegisterClient(string phoneNumber, string email, string fullName, string password)
        {
            if (!_clientService.CheckClient(phoneNumber))
            {
                _clientService.AddClient(phoneNumber, email, fullName, password);
                TempData["SuccessMessage"] = "Account registered successfully. You can now log in.";
                return RedirectToAction("Register");
            }

            TempData["ErrorMessage"] = "Account with this phone number already exists.";
            return RedirectToAction("Register");
        }

    }

}