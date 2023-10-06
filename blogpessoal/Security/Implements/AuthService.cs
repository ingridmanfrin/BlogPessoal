using blogpessoal.Data;
using blogpessoal.Model;
using blogpessoal.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace blogpessoal.Security.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserLogin?> Autenticar(UserLogin userLogin)
        {
            string FotoDefault = "https://i.imgur.com/I8MfmC8.png";

            //verificam
            if (userLogin is null || string.IsNullOrEmpty(userLogin.Usuario) || string.IsNullOrEmpty(userLogin.Senha))
            {
                return null;
            }

            //usuarioLogin.Usuario == email
            //verificam
            var BuscaUsuario = await _userService.GetByUsuario(userLogin.Usuario);

            if (BuscaUsuario is null)
            {
                return null;
            }

            //usuarioLogin.Senha = 123456789
            //BuscaUsuario.Senha = dudbuedbubjqlsmzaojsowwd3f43f.jkifff
            //verifica se as senhas batem
            if(!BCrypt.Net.BCrypt.Verify(userLogin.Senha, BuscaUsuario.Senha))
            {
                return null;
            }

            //Handeler que vai construir o token 
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userLogin.Usuario)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);  

            userLogin.Id = BuscaUsuario.Id;
            userLogin.Nome = BuscaUsuario.Nome;
            userLogin.Foto = BuscaUsuario.Foto is null ? FotoDefault : BuscaUsuario.Foto;
            //WriteToken: vai descerializar o token
            userLogin.Token = "Bearer " + tokenHandler.WriteToken(token).ToString();
            userLogin.Senha = "";

            return userLogin;
        }
    }
}
