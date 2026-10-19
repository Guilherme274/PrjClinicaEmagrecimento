using System.Text;
using System;
using System.Security.Cryptography;


namespace PrjIMV_MVC_CORE.Models.UsuarioModel
{
    public static class MyHash
    {
        public static string GerarSha256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
                byte[] hash = sha256.ComputeHash(bytes);

                return Convert.ToHexString(hash);  
            }
        }
    }
}
