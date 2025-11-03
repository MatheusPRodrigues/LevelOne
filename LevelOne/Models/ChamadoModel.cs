using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LevelOne.Enums;

namespace LevelOne.Models;

public class ChamadoModel
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O Título é obrigatório!")]
    [StringLength(100)]
    [DisplayName("Título")]
    public string Titulo { get; set; }
    
    [Required(ErrorMessage = "A descrição é obrigatória!")]
    [StringLength(2000)]
    [DisplayName("Descrição")]
    public string Descricao { get; set; }
    
    [DisplayName("Data de Abertura")]
    public DateTime DataAbertura { get; set; } = DateTime.Now;
    
    [DisplayName("Data de Encerramento")]
    public DateTime? DataEncerramento { get; set; }
    
    [DisplayName("Urgência")]
    public bool Urgencia { get; set; }

    [Required]
    [DisplayName("Status do Chamado")]
    public StatusEnum StatusChamado { get; set; } = StatusEnum.Aberto;
    
    [Required]
    public int IdCliente { get; set; }
    [ForeignKey("Cliente")]
    public UsuarioModel Cliente { get; set; }

    [ForeignKey("Cliente")]
    public int? IdTecnico { get; set; }
    public UsuarioModel? Tecnico { get; set; }
    
    public List<MensagemModel> Mensagens { get; set; } = new List<MensagemModel>();
}