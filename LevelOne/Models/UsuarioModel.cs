using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required, PasswordPropertyText, MinLength(8)]
        public string Senha { get; set; }

        public bool Ativo { get; set; } = true;

        [NotMapped]
        public List<UsuarioPermissaoModel> UsuarioPermissoes { get; set; } = new List<UsuarioPermissaoModel>();
    }
}
