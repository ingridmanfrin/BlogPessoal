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
        //Buscar por todos
        public async Task<IEnumerable<Postagem>> GetAll()
        {
            //Postagens é meu DbSet
            return await _context.Postagens
                .Include(postagem => postagem.Tema)
                .ToListAsync();   //await: fica esperando uma resposta enquanto as outras coisas rodam
        }

        //metodo async: não vai bloquear meu codigo ewnquanto eu faço a busca
        public async Task<Postagem?> GetById(long id)
        {
            try
            {
                var PostagemUpdate = await _context.Postagens
                    .Include(postagem => postagem.Tema)
                    .FirstAsync(i => i.Id == id);
                
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
                         .Include(postagem => postagem.Tema)
                         .Where(p => p.Titulo.Contains(titulo))
                         .ToListAsync();
            return Postagem;
        }

        public async Task<Postagem?> Create(Postagem postagem)
        {
            if(postagem.Tema is not null)
            {
                var BuscaTema = await _context.Temas.FindAsync(postagem.Tema.Id); 
                
                if(BuscaTema is null)
                {
                    return null;
                }
                
                postagem.Tema = BuscaTema;
            }
            //postagem.Tema = postagem.Tema is not null ? _context.Temas.FirstOrDefault(t => t.Id == postagem.Tema.Id) : null;

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

            if (postagem.Tema is not null)
            {
                var BuscaTema = await _context.Temas.FindAsync(postagem.Tema.Id);

                if (BuscaTema is null)
                {
                    return null;
                }
            }

            postagem.Tema = postagem.Tema is not null ? _context.Temas.FirstOrDefault(t => t.Id == postagem.Tema.Id) : null;

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
