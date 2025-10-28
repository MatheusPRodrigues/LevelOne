using System.Security.Cryptography;
using System.Text;

namespace LevelOne.Helpers
{
    public static class SenhaHelper
    {
        public static string GerarHashParaSenha(string senha)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(senha);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerificarSenha(string senhaDigitada, string hashArmazenado)
        {
            var hash = GerarHashParaSenha(senhaDigitada);  
            return hash == hashArmazenado;
        }
    }
}
