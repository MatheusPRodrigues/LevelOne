using System.Security.Claims;
using LevelOne.Data;
using LevelOne.Enums;
using LevelOne.Models;
using LevelOne.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelOne.Controllers;

public class ChamadosController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChamadosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    // public IActionResult Index()
    // {
    //     return View();
    // }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Create(ChamadoModel chamado)
    {

        // Pega o id do usuário autenticado via cookie/claims
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            TempData["Erro"] = "Usuário não autenticado.";
            return RedirectToAction("Index", "Login");
        }

        chamado.IdCliente = int.Parse(claim.Value);

        ModelState.Remove(nameof(chamado.Cliente));


        if (!ModelState.IsValid)
        {
            return View(chamado);
        }


        _context.Chamados.Add(chamado);
        _context.SaveChanges();

        TempData["Sucesso"] = "Chamado criado com sucesso!";
        return RedirectToAction("Index", "Cliente");
    }

    public async Task<IActionResult> ExibirChamadosClientes()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            TempData["Erro"] = "Usuário não autenticado.";
            return RedirectToAction("Index", "Login");
        }

        int idCliente = int.Parse(claim.Value);

        var chamados = await _context.Chamados
            .Where(c => c.IdCliente == idCliente)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();

        return View(chamados);
    }

    // 🔹 TÉCNICO: Lista chamados abertos, em andamento e finalizados
    public async Task<IActionResult> ExibirChamadosTecnicos()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            TempData["Erro"] = "Usuário não autenticado.";
            return RedirectToAction("Index", "Login");
        }

        int idTecnico = int.Parse(claim.Value);

        // Chamados abertos (sem técnico)
        var chamadosAbertos = await _context.Chamados
            .Where(c => c.IdTecnico == null && c.StatusChamado == StatusEnum.Aberto)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();

        // Chamados em andamento (do técnico logado)
        var chamadosEmAndamento = await _context.Chamados
            .Where(c => c.IdTecnico == idTecnico && c.StatusChamado == StatusEnum.EmAtendimento)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();

        // Chamados finalizados (do técnico logado)
        var chamadosFinalizados = await _context.Chamados
            .Where(c => c.IdTecnico == idTecnico && c.StatusChamado == StatusEnum.Finalizado)
            .OrderByDescending(c => c.DataEncerramento)
            .ToListAsync();

        ViewBag.ChamadosAbertos = chamadosAbertos;
        ViewBag.ChamadosEmAndamento = chamadosEmAndamento;
        ViewBag.ChamadosFinalizados = chamadosFinalizados;

        return View("ExibirChamadosTecnicos");
    }

    // 🔹 TÉCNICO: Assumir um chamado aberto
    [HttpPost]
    public async Task<IActionResult> Assumir(int id)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            TempData["Erro"] = "Usuário não autenticado.";
            return RedirectToAction("Index", "Login");
        }

        int idTecnico = int.Parse(claim.Value);

        var chamado = await _context.Chamados.FindAsync(id);
        if (chamado == null)
            return NotFound();

        if (chamado.IdTecnico != null)
        {
            TempData["Erro"] = "Este chamado já foi assumido por outro técnico.";
            return RedirectToAction("ExibirChamadosTecnicos");
        }

        chamado.IdTecnico = idTecnico;
        chamado.StatusChamado = StatusEnum.EmAtendimento;

        _context.Update(chamado);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Chamado assumido com sucesso!";
        return RedirectToAction("ExibirChamadosTecnicos");
    }

    // 🔹 Detalhes do chamado (restrito para técnicos que assumiram ou cliente dono)
    public async Task<IActionResult> Detalhes(int id)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            TempData["Erro"] = "Usuário não autenticado.";
            return RedirectToAction("Index", "Login");
        }

        int userId = int.Parse(claim.Value);
        bool isTecnico = User.IsInRole("Tecnico");
        bool isCliente = User.IsInRole("Cliente");

        var chamado = await _context.Chamados
            .Include(c => c.Mensagens).ThenInclude(m => m.Usuario)
            .Include(c => c.Cliente)
            .Include(c => c.Tecnico)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chamado == null)
            return NotFound();

        // Permissões:
        if (isCliente && chamado.IdCliente != userId)
        {
            TempData["Erro"] = "Você não tem permissão para visualizar este chamado.";
            return RedirectToAction("MeusChamadosCliente");
        }

        if (isTecnico && chamado.IdTecnico != userId)
        {
            TempData["Erro"] = "Você só pode visualizar chamados que você assumiu.";
            return RedirectToAction("ExibirChamadosTecnicos");
        }

        return View("Detalhes", chamado);
    }

    [HttpPost]
    public async Task<IActionResult> EnviarMensagem([FromBody] MensagemDTO mensagemDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userRole = User.IsInRole("Tecnico") ? "Técnico" : "Cliente";

            var mensagem = new MensagemModel
            {
                ChamadoId = mensagemDto.ChamadoId,
                UsuarioId = userId,
                TipoUsuario = userRole,
                Texto = mensagemDto.Texto,
                DataMensagem = DateTime.Now
            };

            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();

            return Json(new
            {
                texto = mensagem.Texto,
                tipoUsuario = mensagem.TipoUsuario,
                dataMensagem = mensagem.DataMensagem.ToString("dd/MM HH:mm")
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = "Erro ao enviar mensagem: " + ex.Message });
        }
    }
}
