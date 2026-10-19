using System.Text.Json.Serialization;

namespace PrjIMV_MVC_CORE.Models.UsuarioModel
{
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
