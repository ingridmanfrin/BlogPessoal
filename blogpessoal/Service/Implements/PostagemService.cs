using blogpessoal.Data;
using blogpessoal.Model;
using Microsoft.EntityFrameworkCore;

namespace blogpessoal.Service.Implements
{
    public class PostagemService : IPostagemService
    {
        //Estamos fazendo uma injeção de dependencia 
        //todos os objetos que forem somente leitura começam com _
        private readonly AppDbContext _context;

        //metodo construtor
        public PostagemService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Postagem>> GetAll()
        {
            //Postagens é meu DbSet
            return await _context.Postagens.ToListAsync();   //await: fica esperando uma resposta enquanto as outras coisas rodam
        }

        //metodo async: não vai bloquear meu codigo ewnquanto eu faço a busca
        public async Task<Postagem?> GetById(long id)
        {
            try
            {
                var PostagemUpdate = await _context.Postagens.FirstAsync(i => i.Id == id);
                
                return PostagemUpdate;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Postagem>> GetByTitulo(string titulo)
        {
           var Postagem = await _context.Postagens
                         .Where(p => p.Titulo.Contains(titulo))
                         .ToListAsync();
            return Postagem;
        }

        public async Task<Postagem?> Create(Postagem postagem)
        {
            //adiciona na fila
            await _context.Postagens.AddAsync(postagem);
            //persiste na fila
            await _context.SaveChangesAsync();

            return postagem;
        }
        public async Task<Postagem?> Update(Postagem postagem)
        {
            var PostagemUpdate = await _context.Postagens.FindAsync(postagem.Id);

            if(PostagemUpdate is null) 
            {
                return null;
            }

            _context.Entry(PostagemUpdate).State = EntityState.Detached;
            _context.Entry(postagem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return postagem;    
        }

        public async Task Delete(Postagem postagem)
        {
            _context.Remove(postagem);
            await _context.SaveChangesAsync();
        }

    }
}
