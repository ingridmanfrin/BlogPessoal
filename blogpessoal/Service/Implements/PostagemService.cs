using blogpessoal.Data;
using blogpessoal.Model;
using Microsoft.EntityFrameworkCore;

namespace blogpessoal.Service.Implements
{
    public class PostagemService : IPostagemService
    {
        //Estamos fazendo uma injeção de dependencia onde ASP.NET que vai criar ?????????????
        //todos os objetos que forem somente leitura começam com _
        private readonly AppDbContext _context;

        public PostagemService(AppDbContext context)
        {
            _context = context;
        }

        public Task<Postagem?> Create(Postagem postagem)
        {
            throw new NotImplementedException();
        }

        public Task<Postagem?> Delete(Postagem postagem)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Postagem>> GetAll()
        {
            //Postagens é meu DbSet
            return await _context.Postagens.ToListAsync();   //await: fica esperando uma resposta enquanto as outras coisas rodam
        }

        public Task<Postagem?> GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Postagem>> GetByTituulo(string titulo)
        {
            throw new NotImplementedException();
        }

        public Task<Postagem?> Update(Postagem postagem)
        {
            throw new NotImplementedException();
        }
    }
}
