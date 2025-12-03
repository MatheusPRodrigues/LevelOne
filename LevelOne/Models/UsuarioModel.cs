using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LevelOne.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Campo Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Máximo de 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo Email é obrigatório ")]
        [EmailAddress(ErrorMessage = "Insira o formato de email válido!")]
        [MaxLength(200, ErrorMessage = "Máximo de 200 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório")]
        [StringLength(11, ErrorMessage = "O CPF precisa ter 11 números")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigátorio!")]
        [MinLength(8, ErrorMessage = "Mínimo de 8 caracteres")]
        [PasswordPropertyText]
        public string Senha { get; set; }

        public bool Ativo { get; set; } = true;

        [NotMapped]
        public List<UsuarioPermissaoModel> UsuarioPermissoes { get; set; } = new();
    }
}
