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
}

