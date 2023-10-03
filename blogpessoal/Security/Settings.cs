namespace blogpessoal.Security
{
    public class Settings
    {
        //configurando a minha chave de token
        private static string secret = "eead6dc67c770bf4fe20ee13e304b3dc109e65d260f20cf21283766f3f6e3dc9";

        public static string Secret { get => secret; set => secret = value; }
    }
}
