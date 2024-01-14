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