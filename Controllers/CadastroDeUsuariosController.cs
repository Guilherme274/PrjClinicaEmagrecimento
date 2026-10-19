using Microsoft.AspNetCore.Mvc;
using PrjIMV_MVC_CORE.Models;
using PrjIMV_MVC_CORE.Models.PessoaModel;
using PrjIMV_MVC_CORE.Models.UsuarioModel;
using System.Text.Json;

namespace PrjIMV_MVC_CORE.Controllers
{
    public class CadastroDeUsuariosController : Controller
    {
        private const string CaminhoArquivo = "usuarios.json";

        private List<Usuario> CarregarLista()
        {
            List<Usuario> lista = new List<Usuario>();

            if (System.IO.File.Exists(CaminhoArquivo))
            {
                string json = System.IO.File.ReadAllText(CaminhoArquivo);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    lista = JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
                }
            }

            if (lista.Count > 0)
            {
                lista.Sort();
                if (int.TryParse(lista[lista.Count - 1].Id, out int ultimoId))
                {
                    Usuario.contador = ultimoId + 1;
                }
            }
            else
            {
                Usuario.contador = 1;
            }

            return lista;
        }

        private void SalvarLista(List<Usuario> lista)
        {
            lista.Sort();
            string json = JsonSerializer.Serialize(lista);
            System.IO.File.WriteAllText(CaminhoArquivo, json);
        }

        private string ObterLogin(CadastroDeUsuariosViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.Login))
                return vm.Login.Trim();

            if (!string.IsNullOrWhiteSpace(vm.Email) && vm.Email.Contains('@'))
                return vm.Email.Split('@')[0].Trim();

            return (vm.Nome ?? "usuario").Trim().ToLowerInvariant().Replace(" ", "");
        }

        [HttpGet]
        public IActionResult Index()
        {
            CadastroDeUsuariosViewModel vm = new CadastroDeUsuariosViewModel();
            try
            {
                vm.ListaUsuarios = CarregarLista();
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = "Erro ao carregar usuários: " + ex.Message;
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Selecionar(string id)
        {
            CadastroDeUsuariosViewModel vm = new CadastroDeUsuariosViewModel();
            try
            {
                List<Usuario> lista = CarregarLista();
                vm.ListaUsuarios = lista;

                Usuario? encontrado = lista.FirstOrDefault(u => u.Id == id);
                if (encontrado != null)
                {
                    vm.Id = encontrado.Id;
                    vm.Nome = encontrado.Nome;
                    vm.Sobrenome = encontrado.Sobrenome;
                    vm.Cpf = encontrado.Cpf?.cpfLocal;
                    vm.Email = encontrado.Email?.emailLocal;
                    vm.Login = encontrado.Login;
                    vm.PerfilDescricao = encontrado.Perfil != null
                        ? encontrado.Perfil.TipoPerfil.ToString()
                        : TipoPerfil.TAdministrador.ToString();
                }
                else
                {
                    TempData["ErrosModal"] = $"Usuário com ID '{id}' não foi encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = "Erro ao selecionar usuário: " + ex.Message;
            }

            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult Cadastrar(CadastroDeUsuariosViewModel vm)
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                lista = CarregarLista();

                if (string.IsNullOrWhiteSpace(vm.Nome) || string.IsNullOrWhiteSpace(vm.Sobrenome))
                {
                    throw new ArgumentException("Erro de sistema: Nome e Sobrenome são obrigatórios!");
                }

                Cpf cpf = new Cpf();
                cpf.cpf = vm.Cpf ?? string.Empty;

                Email email = new Email();
                email.email = vm.Email ?? string.Empty;

                Perfil perfil = new Perfil(TipoPerfil.TAdministrador);
                string login = ObterLogin(vm);

                Usuario novoUsuario = new Usuario(
                    vm.Nome.Trim(),
                    vm.Sobrenome.Trim(),
                    cpf,
                    email,
                    perfil,
                    login
                );

                lista.Add(novoUsuario);
                SalvarLista(lista);

                TempData["AvisosModal"] = $"Usuário '{novoUsuario.Nome} {novoUsuario.Sobrenome}' (ID: {novoUsuario.Id}) cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = ex.Message;
                vm.ListaUsuarios = lista;
                return View("Index", vm);
            }
        }

        [HttpPost]
        public IActionResult Buscar(CadastroDeUsuariosViewModel vm)
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                lista = CarregarLista();

                string termo = (vm.TermoBusca ?? string.Empty).Trim();
                string cpfBusca = Cpf.Formata(vm.Cpf ?? string.Empty);

                Usuario? encontrado = null;

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    string termoCpf = Cpf.Formata(termo);
                    encontrado = lista.FirstOrDefault(u =>
                        string.Equals(u.Id, termo, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrEmpty(termoCpf) && u.Cpf?.cpfLocal == termoCpf) ||
                        (u.Cpf?.cpfLocal != null && u.Cpf.cpfLocal.Contains(termo, StringComparison.OrdinalIgnoreCase)) ||
                        (u.Email?.emailLocal != null && u.Email.emailLocal.Contains(termo, StringComparison.OrdinalIgnoreCase)) ||
                        $"{u.Nome} {u.Sobrenome}".Contains(termo, StringComparison.OrdinalIgnoreCase));
                }
                else if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    encontrado = lista.FirstOrDefault(u => u.Id == vm.Id.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(vm.Cpf))
                {
                    encontrado = lista.FirstOrDefault(u =>
                        u.Cpf?.cpfLocal == cpfBusca || u.Cpf?.cpfLocal == vm.Cpf.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(vm.Email))
                {
                    encontrado = lista.FirstOrDefault(u =>
                        string.Equals(u.Email?.emailLocal, vm.Email.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                else if (!string.IsNullOrWhiteSpace(vm.Nome))
                {
                    encontrado = lista.FirstOrDefault(u =>
                        $"{u.Nome} {u.Sobrenome}".Contains(vm.Nome.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    throw new ArgumentException("Informe o CPF, Email, Nome, ID ou um termo de pesquisa para buscar!");
                }

                if (encontrado != null)
                {
                    ModelState.Clear();
                    vm.Id = encontrado.Id;
                    vm.Nome = encontrado.Nome;
                    vm.Sobrenome = encontrado.Sobrenome;
                    vm.Cpf = encontrado.Cpf?.cpfLocal;
                    vm.Email = encontrado.Email?.emailLocal;
                    vm.Login = encontrado.Login;
                    vm.PerfilDescricao = encontrado.Perfil != null
                        ? encontrado.Perfil.TipoPerfil.ToString()
                        : TipoPerfil.TAdministrador.ToString();

                    TempData["AvisosModal"] = $"Usuário '{encontrado.Nome} {encontrado.Sobrenome}' (ID: {encontrado.Id}) localizado!";
                }
                else
                {
                    TempData["ErrosModal"] = "Nenhum usuário encontrado com os dados informados!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = "Erro na busca: " + ex.Message;
            }

            vm.ListaUsuarios = lista;
            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult Editar(CadastroDeUsuariosViewModel vm)
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                lista = CarregarLista();

                string cpfFormatado = Cpf.Formata(vm.Cpf ?? string.Empty);

                int index = -1;
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    index = lista.FindIndex(u => u.Id == vm.Id.Trim());
                }
                if (index == -1 && !string.IsNullOrWhiteSpace(cpfFormatado))
                {
                    index = lista.FindIndex(u => u.Cpf?.cpfLocal == cpfFormatado);
                }

                if (index == -1)
                {
                    throw new ArgumentException("Usuário não encontrado para edição! Busque ou selecione um usuário primeiro.");
                }

                if (string.IsNullOrWhiteSpace(vm.Nome) || string.IsNullOrWhiteSpace(vm.Sobrenome))
                {
                    throw new ArgumentException("Erro de sistema: Nome e Sobrenome são obrigatórios!");
                }

                Cpf cpf = new Cpf();
                cpf.cpf = vm.Cpf ?? string.Empty;

                Email email = new Email();
                email.email = vm.Email ?? string.Empty;

                Perfil perfil = new Perfil(TipoPerfil.TAdministrador);
                string idOriginal = lista[index].Id;
                string login = !string.IsNullOrWhiteSpace(vm.Login)
                    ? vm.Login.Trim()
                    : (lista[index].Login ?? ObterLogin(vm));

                int contadorAnterior = Usuario.contador;
                Usuario usuarioAtualizado = new Usuario(
                    vm.Nome.Trim(),
                    vm.Sobrenome.Trim(),
                    cpf,
                    email,
                    perfil,
                    login
                );
                usuarioAtualizado.Id = idOriginal;
                Usuario.contador = contadorAnterior;

                lista[index] = usuarioAtualizado;
                SalvarLista(lista);

                TempData["AvisosModal"] = $"Usuário ID '{idOriginal}' atualizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = ex.Message;
                vm.ListaUsuarios = lista;
                return View("Index", vm);
            }
        }

        [HttpPost]
        public IActionResult Deletar(CadastroDeUsuariosViewModel vm, string? id)
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                lista = CarregarLista();

                string idAlvo = !string.IsNullOrWhiteSpace(id) ? id.Trim() : (vm.Id ?? string.Empty).Trim();
                string cpfFormatado = Cpf.Formata(vm.Cpf ?? string.Empty);

                Usuario? usuarioRemover = null;
                if (!string.IsNullOrWhiteSpace(idAlvo))
                {
                    usuarioRemover = lista.FirstOrDefault(u => u.Id == idAlvo);
                }
                if (usuarioRemover == null && !string.IsNullOrWhiteSpace(cpfFormatado))
                {
                    usuarioRemover = lista.FirstOrDefault(u => u.Cpf?.cpfLocal == cpfFormatado);
                }

                if (usuarioRemover == null)
                {
                    throw new ArgumentException("Usuário não encontrado para deleção! Busque ou selecione um usuário válido.");
                }

                lista.Remove(usuarioRemover);
                SalvarLista(lista);

                TempData["AvisosModal"] = $"Usuário '{usuarioRemover.Nome} {usuarioRemover.Sobrenome}' (ID: {usuarioRemover.Id}) deletado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrosModal"] = ex.Message;
                vm.ListaUsuarios = lista;
                return View("Index", vm);
            }
        }
    }
}
