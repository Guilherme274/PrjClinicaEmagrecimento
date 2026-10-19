using PrjIMV_MVC_CORE.Models.UsuarioModel;

namespace PrjIMV_MVC_CORE.Models
{
    public class CadastroDeUsuariosViewModel
    {
        public Usuario? usuario { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public List<Usuario> Lista { get; set; } = new List<Usuario>();
    }
}
