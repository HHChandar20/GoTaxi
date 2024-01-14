using Microsoft.AspNetCore.Mvc;

public class TaxiController : Controller
{
    public IActionResult Taxi()
    {
        return View();
    }
}