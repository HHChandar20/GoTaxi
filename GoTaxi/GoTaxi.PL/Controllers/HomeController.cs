using Microsoft.AspNetCore.Mvc;
using GoTaxi.BLL.Interfaces;

public class HomeController : Controller
{
    private readonly IDriverService _driverService;

    public HomeController(IDriverService driverService)
    {
        _driverService = driverService;
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
    public IActionResult LoginDriver(string plateNumber)
    {
        bool isAuthenticated = _driverService.CheckDriver(plateNumber);

        if (isAuthenticated)
        {
            return RedirectToAction("Taxi", "Taxi");
        }
        else
        {
            // Return a message indicating account not found
            return Json(new { success = false, message = "Account not found" });
        }
    }

    [HttpPost]
    public IActionResult RegisterDriver(string plateNumber, string username, string email, string password)
    {
        ViewBag.ErrorMessage = "";

        bool isExisting = _driverService.CheckDriver(plateNumber);

        if (!isExisting)
        {
            _driverService.AddDriver(plateNumber, username, email, password);
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