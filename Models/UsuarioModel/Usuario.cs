using PrjIMV_MVC_CORE.Models.PessoaModel;
using System.Text.Json.Serialization;

namespace PrjIMV_MVC_CORE.Models.UsuarioModel
{
    [Serializable]
    public class Usuario : Pessoa, IComparable<Usuario>
    {
        [JsonInclude]
        public String Id;
        [JsonInclude]
        public String Login;
        [JsonInclude]
        public String Hash;
        [JsonInclude]
        public Perfil Perfil;

        public static int contador = 1;
        public Usuario(string nome, 
            string sobrenome, 
            Cpf cpf, 
            Email email,
            Perfil perfil,
            String login) : base(nome, sobrenome, cpf, email)
        {
            try
            {
                Id = (contador++).ToString("D4");
                Perfil = perfil;
                Login = login;
                Hash = MyHash.GerarSha256(cpf.cpf);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ValidaUsuarioSenha(String login, String Senha)
        {
            if (login != Login || Senha == null) return false;
            String var = MyHash.GerarSha256(Senha);
            return var == Hash;
        }
        public int CompareTo(Usuario other)
        {
            return  other != null ? this.Id.CompareTo(other.Id) : -1;
        }
    }
}
