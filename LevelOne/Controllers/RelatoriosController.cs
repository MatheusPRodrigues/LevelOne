using System.Security.Claims;
using LevelOne.Data;
using LevelOne.Enums;
using LevelOne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelOne.Controllers;

public class RelatoriosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RelatorioPdfService _pdfService;
    private readonly RelatorioXlsService _xlsService;

    public RelatoriosController(
        ApplicationDbContext context,
        RelatorioPdfService pdfService,
        RelatorioXlsService xlsService
    )
    {
        _context = context;
        _pdfService = pdfService;
        _xlsService = xlsService;
    }

    public IActionResult Index()
    {
        // Exemplo: ID do técnico logado (ideal pegar via autenticação)
        var tecnicoLogadoId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var chamados = _context.Chamados
            .Include(c => c.Cliente)
            .Include(c => c.Tecnico)
            .ToList();

        var totalAbertos = chamados.Count(c => c.StatusChamado == StatusEnum.Aberto);
        var totalAndamento = chamados.Count(c => c.StatusChamado == StatusEnum.EmAtendimento);
        var totalFinalizados = chamados.Count(c => c.StatusChamado == StatusEnum.Finalizado);
        var totalAtendidosPeloTecnico = chamados.Count(c => c.IdTecnico == tecnicoLogadoId);

        var dados = new
        {
            TotalAbertos = totalAbertos,
            TotalAndamento = totalAndamento,
            TotalFinalizados = totalFinalizados,
            TotalAtendidosPeloTecnico = totalAtendidosPeloTecnico
        };

        return View(dados);
    }
    
    [HttpGet]
    public async Task<IActionResult> RelatorioChamadoEmPdf()
    {
        var chamados = await _context.Chamados
            .Include(c => c.Cliente)
            .Include(c => c.Tecnico)
            .ToListAsync();

        var bytes = _pdfService.GerarRelatorioEmPdf(chamados);

        return File(bytes, "application/pdf", "RelatoriosChamads.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> RelatorioChamadoEmXls()
    {
        var chamados = await _context.Chamados
            .Include(c => c.Cliente)
            .Include(c => c.Tecnico)
            .ToListAsync();

        var bytes = _xlsService.GerarRelatorioEmXls(chamados);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "RelatorioChamados.xlsx");
    }
}