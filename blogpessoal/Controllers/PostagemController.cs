using blogpessoal.Model;
using blogpessoal.Service;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace blogpessoal.Controllers
{
    [Route("~/postagens")]
    [ApiController]
    public class PostagemController : ControllerBase
    {
        private readonly IPostagemService _postagemService;
        private readonly IValidator<Postagem> _postagemValidator;

        public PostagemController(IPostagemService postagemService, IValidator<Postagem> postagemValidator)
        {
            _postagemService = postagemService;
            _postagemValidator = postagemValidator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _postagemService.GetAll());
        }

        //"{id}": é uma variável 
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

            await _postagemService.Create(postagem);

            return CreatedAtAction(nameof(GetById), new {id = postagem.Id}, postagem);    
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Postagem postagem)
        {
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

            if(Resposta is null)
            {
                return NotFound();
            }
            return Ok(Resposta);
        }

        [HttpDelete("{id}")]
        //usamos interface pq delite retorna void
        public async Task <IActionResult> Delete(long id)
        {
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
