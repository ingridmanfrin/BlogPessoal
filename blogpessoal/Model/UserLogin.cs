using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blogpessoal.Model
{
    //não vai gerar uma tabela de banco de dados. essa UserLogin vai ser só uma tabela auxiliar
    public class UserLogin
    {
        public long Id { get; set; }
       
        public string Nome { get; set; } = string.Empty;
   
        public string Usuario { get; set; } = string.Empty;
   
        public string Senha { get; set; } = string.Empty;
     
        public string Foto { get; set; } = string.Empty;
        
        //token não vai ser gravado no banco de dados
        public string Token { get; set; } = string.Empty;

    }
}
