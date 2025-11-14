namespace LevelOne.Models
{
    public class UsuarioPermissaoModel
    {
        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; }
        public int PermissaoId { get; set; }
        public PermissaoModel Permissao { get; set; }

        public UsuarioPermissaoModel(int usuarioId, int permissaoId)
        {
            this.UsuarioId = usuarioId;
            this.PermissaoId = permissaoId;
        }
        
    }
}
