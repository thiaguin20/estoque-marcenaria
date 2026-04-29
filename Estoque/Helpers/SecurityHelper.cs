using System.Security.Cryptography;
using System.Text;

namespace Estoque.Helpers
{
    public static class SecurityHelper
    {
        public static string HashSHA256(string senhaPura)
        {
            if (string.IsNullOrEmpty(senhaPura))
            {
                return string.Empty;
            }

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senhaPura));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static bool VerificarSenha(string senhaDigitada, string hashSalvo)
        {
            if (string.IsNullOrWhiteSpace(hashSalvo))
            {
                return false;
            }

            string hashDigitado = HashSHA256(senhaDigitada);
            return hashDigitado == hashSalvo;
        }
    }
}
