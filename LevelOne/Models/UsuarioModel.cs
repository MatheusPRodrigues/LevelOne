using System.ComponentModel.DataAnnotations;

namespace LevelOne.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required, MaxLength(200)]
        public string Nome { get; set; }

        [Required, EmailAddress, MaxLength(200)]
        public string email { get; set; }

        [Required, StringLength(11)]
        public string Cpf { get; set; }

        [Required, MinLength(8)]
        public string Senha { get; set; }

        public bool Ativo { get; set; }

        [Required]
        public string Roles { get; set; }
    }
}
