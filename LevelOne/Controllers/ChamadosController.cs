using System.Security.Claims;
using LevelOne.Data;
using LevelOne.Enums;
using LevelOne.Models;
using LevelOne.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Cliente")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
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
        return RedirectToAction("ExibirChamadosClientes");
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet]
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

    [Authorize(Roles = "Tecnico")]
    [HttpGet]
    public async Task<IActionResult> ExibirChamadosTecnicos()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            TempData["Erro"] = "Usuário não autenticado.";
            return RedirectToAction("Index", "Login");
        }

        int idTecnico = int.Parse(claim.Value);

        var chamadosAbertos = await _context.Chamados
            .Include(c => c.Cliente)
            .Where(c => c.IdTecnico == null && c.StatusChamado == StatusEnum.Aberto)
            .OrderByDescending(c => c.Urgencia)
            .ThenBy(c => c.DataAbertura)
            .ToListAsync();

        var chamadosEmAndamento = await _context.Chamados
            .Include(c => c.Cliente)
            .Where(c => c.IdTecnico == idTecnico && c.StatusChamado == StatusEnum.EmAtendimento)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();

        var chamadosFinalizados = await _context.Chamados
            .Include(c => c.Cliente)
            .Where(c => c.IdTecnico == idTecnico && c.StatusChamado == StatusEnum.Finalizado)
            .OrderByDescending(c => c.DataEncerramento)
            .ToListAsync();

        ViewBag.ChamadosAbertos = chamadosAbertos;
        ViewBag.ChamadosEmAndamento = chamadosEmAndamento;
        ViewBag.ChamadosFinalizados = chamadosFinalizados;

        return View("ExibirChamadosTecnicos");
    }

    [Authorize(Roles = "Tecnico")]
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
    
    [Authorize(Roles = "Tecnico,Cliente")]
    [HttpGet]
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

    [Authorize(Roles = "Tecnico,Cliente")]
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
    
    [Authorize(Roles = "Tecnico")]
    [HttpPost]
    public async Task<IActionResult> FinalizarChamado(int id)
    {
        var chamado = await _context.Chamados.FindAsync(id);
        if (chamado == null)
        {
            TempData["Erro"] = "Chamado não encontrado.";
            return RedirectToAction("ExibirChamadosTecnicos");
        }

        if (chamado.StatusChamado == StatusEnum.Finalizado)
        {
            TempData["Erro"] = "Esse chamado já foi finalizado.";
            return RedirectToAction("Detalhes", new { id = chamado.Id });
        }

        chamado.StatusChamado = StatusEnum.Finalizado;
        chamado.DataEncerramento = DateTime.Now;

        _context.Chamados.Update(chamado);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Chamado finalizado com sucesso!";
        return RedirectToAction("ExibirChamadosTecnicos");
    }
}
