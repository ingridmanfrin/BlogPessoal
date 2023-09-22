using blogpessoal.Model;

namespace blogpessoal.Service
{
    public interface IPostagemService
    {
        //Usamos o Task porque queremos usar a interação assincrona
        Task<IEnumerable<Postagem>>GetAll();
        Task<Postagem?>GetById(long id);
        Task<IEnumerable<Postagem>> GetByTituulo(string titulo);
        Task<Postagem?> Create(Postagem postagem);
        Task<Postagem?> Update(Postagem postagem);
        Task<Postagem?> Delete(Postagem postagem);
    }
}
