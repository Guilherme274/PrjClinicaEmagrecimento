using PrjIMV_MVC_CORE.Models.UsuarioModel;

namespace PrjIMV_MVC_CORE.Models
{
    public class CadastroDeUsuariosViewModel
    {
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public string? Login { get; set; }
        public string PerfilDescricao { get; set; } = TipoPerfil.TAdministrador.ToString();

        public string? TermoBusca { get; set; }
        public List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
    }
}
