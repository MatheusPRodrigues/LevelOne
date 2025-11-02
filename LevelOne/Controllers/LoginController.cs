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
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext context;

        public LoginController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
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
                return View("Index");
            }

            var permissoes = await context.UsuariosPermissoes
                .Where(ur => ur.UsuarioId == usuario.Id)
                .Select(ur => ur.Permissao.Nome)
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            };

            foreach (var permissao in permissoes)
            {
                claims.Add(new Claim(ClaimTypes.Role, permissao));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            if (permissoes.Contains("Admin"))
                return RedirectToAction("Index", "Admin");
            else if (permissoes.Contains("Tecnico"))
                return RedirectToAction("Index", "Home");
            else if (permissoes.Contains("Cliente"))
                return RedirectToAction("Index", "Cliente");
            else
                return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}
