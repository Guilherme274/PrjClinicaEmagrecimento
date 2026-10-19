using System.Text.Json.Serialization;

namespace PrjIMV_MVC_CORE.Models.PessoaModel
{
    [Serializable]
    public class Cpf
    {
        [JsonInclude]
        public String cpfLocal = "";

        [JsonIgnore]
        public String cpf {
            get { return cpfLocal; } 
            set  
            {                
                if (!Valida(value))
                    throw new ArgumentException($"Erro de sistema: O CPF passado '{value}' é Inválido!");
                cpfLocal = Formata(value);                
            } 
        } 

        public Cpf() 
        {
            cpfLocal = "";
        }       

        public static string Formata(String cpf)
        {
            cpf = cpf.Replace(".", "")
                     .Replace("-", "")
                     .Trim();

            ulong valor;

            if (ulong.TryParse(cpf, out valor))
            {
                return valor.ToString(@"000\.000\.000\-00");
            }
            return string.Empty;
        }

        public static bool Valida(String cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            int d1 = (soma * 10) % 11;
            if (d1 == 10) d1 = 0;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            int d2 = (soma * 10) % 11;
            if (d2 == 10) d2 = 0;

            return cpf[9] - '0' == d1 && cpf[10] - '0' == d2;
        }
    }
}
