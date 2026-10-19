using Microsoft.AspNetCore.Mvc;
using PrjIMV_MVC_CORE.Models;
using PrjIMV_MVC_CORE.Models.PessoaModel;
using PrjIMV_MVC_CORE.Models.UsuarioModel;
using System.Text.Json;

namespace PrjIMV_MVC_CORE.Controllers
{
    public class CadastroDeUsuariosController : Controller
    {
        public Usuario? usuario;

        public List<Usuario>? lista;

        public CadastroDeUsuariosController()
        {
            if (lista == null)
            {
                lista = new List<Usuario>();
            }
        }

        private void CarregarArquivo()
        {
            if (System.IO.File.Exists("usuarios.json"))
            {
                String json = System.IO.File.ReadAllText("usuarios.json");
                lista = JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
                if (lista.Count > 0)
                {
                    lista.Sort();
                    Usuario.contador = int.Parse(lista[lista.Count - 1].Id) + 1;
                }
            }
        }

        public IActionResult Index()
        {
            CadastroDeUsuariosViewModel vm = new CadastroDeUsuariosViewModel();
            CarregarArquivo();
            vm.Lista = lista ?? new List<Usuario>();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Cadastrar(CadastroDeUsuariosViewModel vm)
        {
            try
            {
                CarregarArquivo();

                Cpf cpf = new Cpf();
                cpf.cpf = vm.Cpf ?? "";

                Email email = new Email();
                email.email = vm.Email ?? "";

                Perfil perfil = new Perfil(TipoPerfil.TAdministrador);

                vm.usuario = new Usuario(
                    vm.Nome ?? "",
                    vm.Sobrenome ?? "",
                    cpf,
                    email,
                    perfil,
                    vm.Nome ?? ""
                );

                if (lista != null)
                {
                    lista.Add(vm.usuario);
                    String json = JsonSerializer.Serialize(lista);
                    System.IO.File.WriteAllText("usuarios.json", json);
                }

                TempData["AvisosModal"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = ex.Message;
                vm.Lista = lista ?? new List<Usuario>();
                return View("Index", vm);
            }
        }

        [HttpPost]
        public IActionResult Deletar(CadastroDeUsuariosViewModel vm)
        {
            try
            {
                CarregarArquivo();

                string cpfFormatado = Cpf.Formata(vm.Cpf ?? "");

                if (lista != null)
                {
                    for (int i = 0; i < lista.Count; i++)
                    {
                        if (lista[i].Cpf.cpf == cpfFormatado)
                        {
                            lista.RemoveAt(i);
                            String json = JsonSerializer.Serialize(lista);
                            System.IO.File.WriteAllText("usuarios.json", json);

                            TempData["AvisosModal"] = "Usuário deletado com sucesso!";
                            return RedirectToAction("Index");
                        }
                    }
                }

                TempData["ErrosModal"] = "Usuário não encontrado para deleção!";
                vm.Lista = lista ?? new List<Usuario>();
                return View("Index", vm);
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = ex.Message;
                vm.Lista = lista ?? new List<Usuario>();
                return View("Index", vm);
            }
        }
    }
}
