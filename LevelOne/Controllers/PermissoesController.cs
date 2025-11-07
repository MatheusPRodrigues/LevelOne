using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LevelOne.Data;
using LevelOne.Models;
using Microsoft.AspNetCore.Authorization;

namespace LevelOne.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermissoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermissoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Permissoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Permissoes.ToListAsync());
        }

        // GET: Permissoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissaoModel = await _context.Permissoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissaoModel == null)
            {
                return NotFound();
            }

            return View(permissaoModel);
        }

        // GET: Permissoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Permissoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] PermissaoModel permissaoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permissaoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permissaoModel);
        }

        // GET: Permissoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissaoModel = await _context.Permissoes.FindAsync(id);
            if (permissaoModel == null)
            {
                return NotFound();
            }
            return View(permissaoModel);
        }

        // POST: Permissoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] PermissaoModel permissaoModel)
        {
            if (id != permissaoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permissaoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissaoModelExists(permissaoModel.Id))
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
            return View(permissaoModel);
        }

        // GET: Permissoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissaoModel = await _context.Permissoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissaoModel == null)
            {
                return NotFound();
            }

            return View(permissaoModel);
        }

        // POST: Permissoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permissaoModel = await _context.Permissoes.FindAsync(id);
            if (permissaoModel != null)
            {
                _context.Permissoes.Remove(permissaoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissaoModelExists(int id)
        {
            return _context.Permissoes.Any(e => e.Id == id);
        }
    }
}
