using System.Text.Json.Serialization;

namespace PrjIMV_MVC_CORE.Models.PessoaModel
{
    [Serializable]
    public abstract class Pessoa : IComparable<Pessoa>
    {
        protected Pessoa(String nome, String sobrenome, Cpf? cpf, Email? email)
        {
            try
            {
                Nome = nome;
                Sobrenome = sobrenome;
                Cpf = cpf==null ? new Cpf(): cpf;
                Email = email == null ? new Email() : email;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public String Nome { get; private set; } = String.Empty;
        public String Sobrenome { get; private set; } = String.Empty;

        [JsonInclude]
        public Cpf Cpf  = new Cpf();
        [JsonInclude]
        public Email Email = new Email();
        public int CompareTo(Pessoa other)
        {
          return Cpf.cpf.CompareTo(other != null && other.Cpf != null ? other.Cpf.cpf : String.Empty);
        }
    }
}