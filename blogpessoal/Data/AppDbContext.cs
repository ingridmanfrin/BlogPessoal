using blogpessoal.Model;
using blogpessoal.Validator;
using Microsoft.EntityFrameworkCore;

namespace blogpessoal.Data
{
    
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //MODEL GERA AS TABELAS
            modelBuilder.Entity<Postagem>().ToTable("tb_postagens");
            modelBuilder.Entity<Tema>().ToTable("tb_temas");
            modelBuilder.Entity<User>().ToTable("tb_usuarios");

            _ = modelBuilder.Entity<Postagem>()
                .HasOne(_ =>_.Tema)                  //indica lado um da relação
                .WithMany(t => t.Postagem)           //indica lado muitos da relação
                .HasForeignKey("TemaId")            //indica foringkey
                .OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<Postagem>()
               .HasOne(_ => _.Usuario)                  //indica lado um da relação
               .WithMany(t => t.Postagem)           //indica lado muitos da relação
               .HasForeignKey("UsuarioId")            //indica foringkey
               .OnDelete(DeleteBehavior.Cascade);
        }

        //Registar DbSet - Objeto responsável por manipular a Tabela Postagem e Temas
        //Seu eu não criar o DbSet eu não consigo fazer CRUD
        public DbSet<Postagem> Postagens { get; set; } = null!;
        public DbSet<Tema> Temas { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var insertedEntries = this.ChangeTracker.Entries()
                                   .Where(x => x.State == EntityState.Added)
                                   .Select(x => x.Entity);

            //insertedEntry: vê se é uma inserção
            foreach (var insertedEntry in insertedEntries)
            {
                //Se uma propriedade da Classe Auditable estiver sendo criada vai ter metodo responssável por persistir a informação
                if (insertedEntry is Auditable auditableEntity)
                {
                    //new TimeSpan(-3,0,0): criamos um novo datime e arrumamos o utc que é -3 horas do de greenwich (com exeção de de alguns estados do Brasil)
                    auditableEntity.Data = new DateTimeOffset(DateTime.Now.ToUniversalTime(), new TimeSpan(-3,0,0));
                }
            }

            var modifiedEntries = ChangeTracker.Entries()
                       .Where(x => x.State == EntityState.Modified)
                       .Select(x => x.Entity);

            foreach (var modifiedEntry in modifiedEntries)
            {
                //Se uma propriedade da Classe Auditable estiver sendo atualizada.  
                if (modifiedEntry is Auditable auditableEntity)
                {
                    auditableEntity.Data = new DateTimeOffset(DateTime.Now.ToUniversalTime(), new TimeSpan(-3, 0, 0)); ;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
