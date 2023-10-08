using blogpessoal.Model;
using blogpessoal.Service;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

//controller: faz verificações()
namespace blogpessoal.Controllers
{
    //[Authorize]: todas as classes precisam de token
    [Authorize]
    [Route("~/postagens")]
    [ApiController]
    public class PostagemController : ControllerBase
    {
        //_postagemService e _postagemValidator são os objetos para acessarmos
        private readonly IPostagemService _postagemService;
        private readonly IValidator<Postagem> _postagemValidator;

        //construtor que vai receber os serviços via injeção de dependencia
        //só vai ser usado quando precisa, então é o C# que vai cuidar disso
        public PostagemController(IPostagemService postagemService, IValidator<Postagem> postagemValidator)
        {
            _postagemService = postagemService;
            _postagemValidator = postagemValidator;
        }

        //async: metodo assíncrono
        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            //await: espera que o resultado da função seja retornado
            return Ok(await _postagemService.GetAll());
        }

        //"{id}": é o parâmetro 
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var Resposta =await _postagemService.GetById(id);

            if(Resposta is null)
            {
                return NotFound();
            }
            return Ok(Resposta);
        }

        //"titulo/{titulo}": variável de caminho
        [HttpGet("titulo/{titulo}")]
        public async Task<ActionResult> GetByTitulo(string titulo)
        {
            return Ok(await _postagemService.GetByTitulo(titulo));
        }

        [HttpPost]
        //postagem: é um objeto, não pode ir na variável de caminho
        //FromBody: é para pegar o objeto portagem no corpo da requisição
        public async Task<ActionResult> Create([FromBody] Postagem postagem)
        {
            var validarPostagem = await _postagemValidator.ValidateAsync(postagem); 

            if(!validarPostagem.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validarPostagem);
            }

            var Resposta = await _postagemService.Create(postagem);

            //if para ter um retorno caso seja colocado um tema que não existe
            if(Resposta is null)
            {
                return BadRequest("Tema não encontrado!");
            }
            //esse return é uma junção de GetById e Created
            return CreatedAtAction(nameof(GetById), new { id = postagem.Id }, postagem);    
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Postagem postagem)
        {
            //verifica se o id não é 0
            if(postagem.Id == 0)
            {
                return BadRequest("Id da Postagem é inválido!");
            }
            
            var validarPostagem = await _postagemValidator.ValidateAsync(postagem);

            if (!validarPostagem.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validarPostagem);
            }

            var Resposta = await _postagemService.Update(postagem);

            //verifica se o tema existe. exemplo não temos o tema 1000 mas o usuário digitou 1000, vai passar pelo primeiro if de id, mas vai cair nesse if
            if(Resposta is null)
            {
                return NotFound("Postagem e/ou Tema não encontrados!");
            }
            return Ok(Resposta);
        }

        [HttpDelete("{id}")]
        //usamos interface pq delite retorna void
        public async Task <IActionResult> Delete(long id)
        {
            //para saber se o id existe 
            var BuscaPostagem = await _postagemService.GetById(id); 

            if(BuscaPostagem is null)
            {
                return NotFound("Postagem não foi encontrada!");
            }
            await _postagemService.Delete(BuscaPostagem);
            return NoContent();
        }

    }
}
