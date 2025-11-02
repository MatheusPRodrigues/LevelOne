using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelOne.Controllers;

[Authorize(Roles = "Cliente")]
public class ClienteController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}