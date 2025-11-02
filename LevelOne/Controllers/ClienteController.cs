using Microsoft.AspNetCore.Mvc;

namespace LevelOne.Controllers;

public class ClienteController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}