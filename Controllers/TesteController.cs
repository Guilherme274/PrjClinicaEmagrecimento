using Microsoft.AspNetCore.Mvc;
using PrjIMV_MVC_CORE.Models;
using PrjIMV_MVC_CORE.Models.PessoaModel;
using PrjIMV_MVC_CORE.Models.UsuarioModel;
using System.Text.Json;

namespace PrjIMV_MVC_CORE.Controllers
{
    public class TesteController : Controller
    {  
        public Usuario? usuario;

        public List<Usuario>? lista;

        public TesteController()
        {
            if (lista == null)
            {
                lista = new List<Usuario>();
            }
        }
        public IActionResult Index()
        {
            try
            {
                if (System.IO.File.Exists("usuarios.json"))
                {
                    String json = System.IO.File.ReadAllText("usuarios.json");
                    lista = JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
                    lista.Sort();
                    Usuario.contador = int.Parse(lista[lista.Count - 1].Id) + 1;
                }             

                TesteViewModel vm = new TesteViewModel();

                Cpf cpf = new Cpf();
                cpf.cpf = "32369280778";
                Email email = new Email();
                Perfil perfil = new Perfil(TipoPerfil.TAdministrador);
                email.email = "halrangel@yahoo.com.br";
                vm.usuario = new Usuario("Helio Augusto",
                    "de Lima Rangel", cpf, email, perfil, "hlrangel");

                if (lista != null)
                {
                    lista.Add(vm.usuario);
                    String json = JsonSerializer.Serialize(lista);
                    System.IO.File.WriteAllText("usuarios.json", json);
                }

                if (vm.usuario.ValidaUsuarioSenha("hlrangel", Cpf.Formata("32369280778")))
                {
                    TempData["AvisosModal"] = "Usuário OK";
                }
                else
                {
                    TempData["ErrosModal"] = "Usuário não OK";
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = "Erro ao serializar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
