using Microsoft.AspNetCore.Mvc;
using GoTaxi.BLL.Interfaces;
using GoTaxi.DAL.Models;
using GoTaxi.BLL.Services;

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
    public IActionResult Login()
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

        if (_driverService.CheckDriver(plateNumber))
        {
            if (_driverService.AuthenticateDriver(plateNumber, password))
            {
                // Store current driver in cookies
                HttpContext.Response.Cookies.Append("CurrentUserType", "Driver");
                HttpContext.Response.Cookies.Append("CurrentPlateNumber", plateNumber);

                return RedirectToAction("Taxi", "Taxi");
            }
            // Return a message indicating account not found
            return Json(new { success = false, message = "Wrong password" });
        }
        else
        {
            // Return a message indicating client not found
            ViewBag.ErrorMessage = "Driver not found!";

            return View("Login");
        }
    }

    [HttpPost]
    public IActionResult LoginClient(string phoneNumber, string password)
    {
        if (_clientService.CheckClient(phoneNumber))
        {

            if (_clientService.AuthenticateClient(phoneNumber, password))
            {
                // Store current client in cookies
                HttpContext.Response.Cookies.Append("CurrentUserType", "Client");
                HttpContext.Response.Cookies.Append("CurrentPhoneNumber", phoneNumber);

                return RedirectToAction("Client", "Client");
            }
            // Return a message indicating account not found
            return Json(new { success = false, message = "Wrong password" });
        }
        else
        {
            // Return a message indicating client is not found
            ViewBag.ErrorMessage = "Client not found!";

            return View("Login");
        }
    }

    [HttpPost]
    public IActionResult RegisterDriver(string plateNumber, string email, string fullName, string password)
    {
        ViewBag.ErrorMessage = "";

        bool isExisting = _driverService.CheckDriver(plateNumber);

        if (!_driverService.CheckDriver(plateNumber))
        {
            _driverService.AddDriver(plateNumber, email, fullName, password);
            return RedirectToAction("Login");
        }
        else
        {
            // Return a message indicating account is already registered
            ViewBag.ErrorMessage = "Driver is already registered";

            return View("Register");
        }
    }

    [HttpPost]
    public IActionResult RegisterClient(string phoneNumber, string email, string fullName, string password)
    {
        ViewBag.ErrorMessage = "";

        if (!_clientService.CheckClient(phoneNumber))
        {
            _clientService.AddClient(phoneNumber, email, fullName, password);
            return RedirectToAction("Login");
        }
        else
        {
            // Return a message indicating account is already registered
            ViewBag.ErrorMessage = "Client is already registered";

            return View("Register");
        }
    }


}