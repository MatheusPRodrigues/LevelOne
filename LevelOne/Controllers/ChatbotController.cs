using LevelOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelOne.Controllers;

[Authorize(Roles = "Cliente")]
public class ChatbotController : Controller
{
    private readonly DialogFlowService _dialogFlowService;

    public ChatbotController(DialogFlowService dialogFlowService)
    {
        _dialogFlowService = dialogFlowService;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> EnviarMensagem(string mensagem)
    {
        if (String.IsNullOrWhiteSpace(mensagem))
            return BadRequest("Mensagem vazia");

        string sessionId = Guid.NewGuid().ToString();

        var resposta = await _dialogFlowService.SendMessageAsync(sessionId, mensagem);

        return Json(new { resposta });
    }
}