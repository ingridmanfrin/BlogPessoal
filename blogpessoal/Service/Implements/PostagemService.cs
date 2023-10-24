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
                .AsNoTracking()
                //.Include : verifica se tem um tema em postagem
                .Include(postagem => postagem.Tema)
                .Include(p => p.Usuario)
                .ToListAsync();   //await: fica esperando uma resposta enquanto as outras coisas rodam
        }

        //metodo async: não vai bloquear meu codigo ewnquanto eu faço a busca
        public async Task<Postagem?> GetById(long id)
        {
            try
            {
                var PostagemUpdate = await _context.Postagens
                    .Include(postagem => postagem.Tema)
                    .Include(p => p.Usuario)
                    .FirstAsync(p => p.Id == id);

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
                          .AsNoTracking()
                          .Include(postagem => postagem.Tema)
                          .Include(p => p.Usuario)
                          .Where(p => p.Titulo.ToUpper()
                                .Contains(titulo.ToUpper())
                           )
                          .ToListAsync();
            return Postagem;
        }

        public async Task<Postagem?> Create(Postagem postagem)
        {
            if (postagem.Tema is not null)
            {
                var BuscaTema = await _context.Temas.FindAsync(postagem.Tema.Id);

                if (BuscaTema is null)
                {
                    return null;
                }

                postagem.Tema = BuscaTema;
            }

            postagem.Usuario = postagem.Usuario is not null ? await _context.Users.FirstOrDefaultAsync(u => u.Id == postagem.Usuario.Id) : null;

            //adiciona na fila
            await _context.Postagens.AddAsync(postagem);
            //persiste na fila
            await _context.SaveChangesAsync();

            return postagem;
        }
        public async Task<Postagem?> Update(Postagem postagem)
        {
            //esses ifs de update do service somente verificam se não é null as validações. A respeito de se ==0 ou se realmente existe um id mesmo fica por conta da Controller!
            var PostagemUpdate = await _context.Postagens.FindAsync(postagem.Id);

            if (PostagemUpdate is null)
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
