using LevelOne.Data;
using LevelOne.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace LevelOne.Controllers
{
    public class ContasController : Controller
    {
        private readonly ApplicationDbContext context;

        public ContasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string cpf, string senha)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf && u.Ativo);

            if (usuario == null || !SenhaHelper.VerificarSenha(senha, usuario.Senha))
            {
                ViewBag.Erro = "CPF ou senha inválidos!";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Roles)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            if (usuario.Roles == "Admin")
                return RedirectToAction("Index", "Admin");
            else if (usuario.Roles == "Tecnico")
                return RedirectToAction("Index", "Tecnico");
            else
                return RedirectToAction("Index", "Cliente");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
