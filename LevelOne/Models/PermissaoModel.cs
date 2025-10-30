using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LevelOne.Models
{
    public class PermissaoModel
    {
        [Key]
        public int Id { get; set; }

        [Required, DisplayName("Permissão")]
        public string Nome { get; set; } = String.Empty;
    }
}
