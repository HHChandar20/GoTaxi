using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GoTaxi.BLL.Interfaces;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAccountService _accountService;

    public HomeController(ILogger<HomeController> logger, IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Taxi()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        bool isAuthenticated = _accountService.AuthenticateAccount(username, password);

        if (isAuthenticated)
        {
            return RedirectToAction("Taxi");
        }
        else
        {
            // Return a message indicating account not found
            return Json(new { success = false, message = "Account not found" });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }
}