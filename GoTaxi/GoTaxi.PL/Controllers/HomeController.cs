using Microsoft.AspNetCore.Mvc;
using GoTaxi.BLL.Interfaces;

public class HomeController : Controller
{
    private readonly IDriverService _driverService;
    private readonly IClientService _clientService;

    public HomeController(IDriverService driverService, IClientService clientService)
    {
        _driverService = driverService;
        _clientService = clientService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult LoginDriver(string plateNumber, string password)
    {
        bool isExisting = _driverService.CheckDriver(plateNumber);

        if (isExisting)
        {
            bool isAuthenticated = _driverService.AuthenticateDriver(plateNumber, password);
            if (isAuthenticated)
            {
                return RedirectToAction("Taxi", "Taxi");
            }
            // Return a message indicating account not found
            return Json(new { success = false, message = "Wrong password" });
        }
        else
        {
            // Return a message indicating account not found
            return Json(new { success = false, message = "Account not found" });
        }
    }

    [HttpPost]
    public IActionResult LoginClient(string phoneNumber, string password)
    {
        bool isExisting = _clientService.CheckClient(phoneNumber);

        if (isExisting)
        {
            bool isAuthenticated = _clientService.AuthenticateClient(phoneNumber, password);
            if (isAuthenticated)
            {
                return RedirectToAction("Client", "Client");
            }
            // Return a message indicating account not found
            return Json(new { success = false, message = "Wrong password" });
        }
        else
        {
            // Return a message indicating account not found
            return Json(new { success = false, message = "Account not found" });
        }
    }

    [HttpPost]
    public IActionResult RegisterClient(string phoneNumber, string email, string fullName, string password)
    {
        ViewBag.ErrorMessage = "";

        bool isExisting = _clientService.CheckClient(phoneNumber);

        if (!isExisting)
        {
            _clientService.AddClient(phoneNumber, email, fullName, password);
            return RedirectToAction("Index");
        }
        else
        {
            // Return a message indicating account is already registered
            ViewBag.ErrorMessage = "Account is already registered";

            // Render the Register view again with the error message
            return View("Register");
        }
    }

    [HttpPost]
    public IActionResult RegisterDriver(string plateNumber, string email, string fullName, string password)
    {
        ViewBag.ErrorMessage = "";

        bool isExisting = _driverService.CheckDriver(plateNumber);

        if (!isExisting)
        {
            _driverService.AddDriver(plateNumber, email, fullName, password);
            return RedirectToAction("Index");
        }
        else
        {
            // Return a message indicating account is already registered
            ViewBag.ErrorMessage = "Account is already registered";

            // Render the Register view again with the error message
            return View("Register");
        }
    }

}