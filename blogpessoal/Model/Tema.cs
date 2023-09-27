using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace blogpessoal.Model
{
    public class Tema
    {
        [Key]  //Primary Key (Id)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // IDENTITY(1,1)
        public long Id { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(1000)]
        public string Descricao { get; set; } = string.Empty;

        //o InverseProperty vai funcionar semelhante ao this
        [InverseProperty("Tema")]
        public virtual ICollection<Postagem>? Postagem { get; set; }    
    }
}
