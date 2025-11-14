using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LevelOne.Models;

public class MensagemModel
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    [ForeignKey("Chamado")]
    public int ChamadoId { get; set; }
    public ChamadoModel Chamado { get; set; }
    
    [Required]
    [ForeignKey("Usuario")]
    public int UsuarioId { get; set; }  
    public UsuarioModel Usuario { get; set; }

    [Required]
    public string TipoUsuario { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "O necessário inserir um texto para mensagem!")]
    [StringLength(2000, ErrorMessage = "A mensagem deve ter no máximo 2000 caracteres")]
    public string Texto { get; set; }
    
    [Required]
    public DateTime DataMensagem { get; set; } = DateTime.Now;
}
