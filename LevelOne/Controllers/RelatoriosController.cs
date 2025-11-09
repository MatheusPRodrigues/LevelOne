using LevelOne.Data;
using LevelOne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelOne.Controllers;

public class RelatoriosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RelatorioPdfService _pdfService;

    public RelatoriosController(ApplicationDbContext context, RelatorioPdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<IActionResult> RelatorioChamadoEmPdf()
    {
        var chamados = await _context.Chamados
            .Include(c => c.Cliente)
            .ToListAsync();

        var bytes = _pdfService.GerarRelatorioEmPdf(chamados);

        return File(bytes, "application/pdf", "RelatoriosChamads.pdf");
    }
}