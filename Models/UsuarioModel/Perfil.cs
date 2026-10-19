using System.Text.Json.Serialization;

namespace PrjIMV_MVC_CORE.Models.UsuarioModel
{
    public static class Permissao
    {       
        private static readonly HashSet<string> PermissoesMedico =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "formDadosPaciente",
            "formCadastroPaciente",            
        };
        private static readonly HashSet<string> PermissoesAdministrador =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "formDadosPaciente",
            "formCadastroUsuario",
            "formCadastroPaciente",
        };
        public static bool OkMedico(string form)
        {
            if (form == null || string.IsNullOrWhiteSpace(form))
                return false;
            return PermissoesMedico.Contains(form);
        }
        public static bool OkAdministrador(string form)
        {
            if (form == null || string.IsNullOrWhiteSpace(form))
                return false;
            return PermissoesAdministrador.Contains(form);
        }
    }
    [Serializable]
    public enum TipoPerfil
    {
        TMedico,
        TPaciente,
        TAdministrador,
        TDesenvolvedor,
        TVisitante,
        TFuncionario
    }
    [Serializable]
    public class Perfil
    {
        [JsonInclude]
        public TipoPerfil TipoPerfil;

        public Perfil(TipoPerfil tipoPerfil)
        {
            TipoPerfil=tipoPerfil;
        }

        public bool ok(string form)
        {
            switch (TipoPerfil)
            {
                case TipoPerfil.TMedico: return Permissao.OkMedico(form);  
                case TipoPerfil.TAdministrador : return Permissao.OkAdministrador(form);  
                case TipoPerfil.TDesenvolvedor: return true;  
                    default: throw new ArgumentException("Erro de sistema: Perfil não existe!");
            }             
        }

        public string nomeForm="";

        [JsonIgnore]
        public bool okMedico
        {
            get
            {   
                return Permissao.OkMedico(nomeForm);
            }
        }
        [JsonIgnore]
        public bool okDesenvolevedor 
        { 
            get
            {
                return true;
            } 
        }
        [JsonIgnore]
        public bool okAdministrador 
        { 
            get
            {
                return Permissao.OkAdministrador(nomeForm);
            } 
        }
        [JsonIgnore]
        public bool okPaciente { get; }

        [JsonIgnore]
        public bool okAtendente { get; }

        
    }
}
