using System.ComponentModel.DataAnnotations;

namespace LevelOne.Enums;

public enum StatusEnum
{
    [Display(Name = "Aberto")]
    Aberto = 1,
    [Display(Name = "Em Atendimento")]
    EmAtendimento = 2,
    [Display(Name = "Finalizado")]
    Finalizado = 3
}