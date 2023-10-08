using blogpessoal.Model;
using BlogPessoalTeste.Factory;
using FluentAssertions;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit.Extensions.Ordering;

namespace blogpessoaltest.Controllers
{
    public class UserControllerTest : IClassFixture<WebAppFactory>
    {
        protected readonly WebAppFactory _factory;
        protected HttpClient _client;

        //dynamic: é um objeto genérico, não tem tipo definido (é mais indicado para testes)
        private readonly dynamic token;
        private string Id { get; set; } = string.Empty;

        public UserControllerTest(WebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            token = GetToken();
        }              
                                                                             
        private static dynamic GetToken()
        {
            dynamic data = new ExpandoObject();
            data.sub = "root@root.com";
            return data;
        }

        [Fact, Order(1)]
        public async Task DeveCriarNovoUsuario()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                { "nome", "João" },
                { "usuario", "joao12@email.com.br" },
                { "senha", "12345678" },
                { "foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);

            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            resposta.EnsureSuccessStatusCode();

            resposta.StatusCode.Should().Be(HttpStatusCode.Created);

        }

        [Fact, Order(2)]
        public async Task DeveDarErroEmail()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                { "nome", "João" },
                { "usuario", "joao12email.com.br" },
                { "senha", "12345678" },
                { "foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);

            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            //resposta.EnsureSuccessStatusCode();

            resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact, Order(3)]
        public async Task NaoDeveCriarUsuarioDuplicado()
        {
            var novoUsuario = new Dictionary<string, string>
            {
                { "nome", "Karina" },
                { "usuario", "karina@email.com.br" },
                { "senha", "123478945" },
                { "foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);

            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            //Enviar a segunda vez

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            //o retorno BadRequest aparece quando o usuário já existe ou quando passa um dado do corpo da requisição de forma incorreta
        }

        [Fact, Order(4)]
        public async Task DeveListarTodosOsUsuarios()
        {
            //object: tratar o formato do token
            _client.SetFakeBearerToken((object)token);

            var resposta = await _client.GetAsync("/usuarios/all");

            resposta.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact, Order(5)]
        public async Task DeveAtualizarUsuario()
        {
            // Criar Usuário
            var novoUsuario = new Dictionary<string, string>()
            {
                { "nome", "Paulo Antunes" },
                { "usuario", "paulo@email.com.br" },
                { "senha", "123478945" },
                { "foto", "" }
            };
            //transformar para json
            var postJson = JsonConvert.SerializeObject(novoUsuario);
            //Encoding.UTF8: caracteres especiais
            var corpoRequisicaoPost = new StringContent(postJson, Encoding.UTF8, "application/json");
            //requisição propriamente dita do cadastro
            var respostaPost = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicaoPost);

            //para conseguir resgatar o id do usuário
            var corpoRespostaPost = await respostaPost.Content.ReadFromJsonAsync<User>();
            if (corpoRespostaPost is not null)
            {
                Id = corpoRespostaPost.Id.ToString();
            }

            //Atualizar Usuário
            var atualizarUsuario = new Dictionary<string, string>
            {
                { "id", Id },
                { "nome", "Paulo Cesar Antunes" },
                { "usuario", "paulo_cesar@email.com.br" },
                { "senha", "12345678" },
                { "foto", "" }
            };

            var updateJson = JsonConvert.SerializeObject(atualizarUsuario);
            //tratar para não ter erros na requisição
            var corpoRequisicaoUpdate = new StringContent(updateJson, Encoding.UTF8, "application/json");

            _client.SetFakeBearerToken((object)token);

            var respostaPut = await _client.PutAsync("/usuarios/atualizar", corpoRequisicaoUpdate);

            respostaPut.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact, Order(6)]
        public async Task DeveListarUmUsuario()
        {
            //object: tratar o formato do token
            _client.SetFakeBearerToken((object)token);

            var resposta = await _client.GetAsync("/usuarios/1");

            resposta.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact, Order(7)]
        public async Task DeveAutenticarUmUsuario()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                { "usuario", "joao12@email.com.br" },
                { "senha", "12345678" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);

            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/logar", corpoRequisicao);

            resposta.EnsureSuccessStatusCode();

            resposta.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
