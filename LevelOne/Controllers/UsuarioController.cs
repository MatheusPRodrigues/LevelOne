using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LevelOne.Data;
using LevelOne.Models;
using LevelOne.Helpers;

namespace LevelOne.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioModel = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioModel == null)
            {
                return NotFound();
            }

            return View(usuarioModel);
        }

        // GET: Usuario/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var permissoes = await _context.Permissoes.ToListAsync();
            ViewBag.Permissoes = permissoes;
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioModel usuarioModel, List<int> permissoesSelecionadas)
        {
            if (ModelState.IsValid)
            {
                usuarioModel.Senha = SenhaHelper.GerarHashParaSenha(usuarioModel.Senha);
                _context.Add(usuarioModel);
                await _context.SaveChangesAsync();

                foreach (var permissao in permissoesSelecionadas)
                {
                    var usuarioPermissao = new UsuarioPermissaoModel(usuarioModel.Id, permissao);
                    _context.UsuariosPermissoes.Add(usuarioPermissao);
                }

                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Permissoes = await _context.Permissoes.ToListAsync();
            return View(usuarioModel);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioModel = await _context.Usuarios
                .Include(u => u.UsuarioPermissoes)
                .FirstOrDefaultAsync(u => u.Id == id);

            ViewBag.Permissoes = await _context.Permissoes .ToListAsync();

            if (usuarioModel == null)
            {
                return NotFound();
            }
            return View(usuarioModel);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Ativo")] UsuarioModel usuarioModel, List<int> permissoes)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario is null)
            {
                return NotFound();
            }

            ModelState.Remove("Cpf");
            ModelState.Remove("Senha");

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Nome = usuarioModel.Nome;
                    usuario.Email = usuarioModel.Email;
                    usuario.Ativo = usuarioModel.Ativo;

                    // Atualiza permissões
                    _context.UsuariosPermissoes.RemoveRange(usuario.UsuarioPermissoes);
                    foreach (var permissao in permissoes)
                    {
                        _context.UsuariosPermissoes.Add(new UsuarioPermissaoModel
                        (
                            usuario.Id,
                            permissao
                        ));
                    }

                    //_context.Update(usuarioModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioModelExists(usuarioModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuarioModel);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioModel = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioModel == null)
            {
                return NotFound();
            }

            return View(usuarioModel);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioModel = await _context.Usuarios.FindAsync(id);
            if (usuarioModel != null)
            {
                _context.Usuarios.Remove(usuarioModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioModelExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
