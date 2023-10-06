
using blogpessoal.Data;
using blogpessoal.Model;
using blogpessoal.Security;
using blogpessoal.Security.Implements;
using blogpessoal.Service;
using blogpessoal.Service.Implements;
using blogpessoal.Validator;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace blogpessoal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //quando for fazer a descerialização do objeto não fazer um loop infinito
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            //Conecção com o Banco de Dados 
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //estou indicando para a minha aplicação qual é a string de conecção que estou usando que puchei lá de appsettings.json

            //estou utilizando a conecção
            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString)
            );

            //Registrar a Validações das Entidades
            //AddTransient: só vai instanciar quando vc precisar, para cadastrar e atualizar
            builder.Services.AddTransient<IValidator<Postagem>, PostagemValidator>();
            builder.Services.AddTransient<IValidator<Tema>, TemaValidator>();
            builder.Services.AddTransient<IValidator<User>, UserValidator>();


            //Registrar as Classes de Serviço (Service)
            //AddScoped: 
            builder.Services.AddScoped<IPostagemService, PostagemService>();
            builder.Services.AddScoped<ITemaService, TemaService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            //validação do Token
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(Settings.Secret);
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Configuração do CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyPolicy",
                    policy =>
                    {
                        policy.AllowAnyOrigin()    //para receber as requisições. Se tivessemos um frontend pronto travaríamos colocando o front entre ()
                        .AllowAnyMethod()          //para recer os delete, get, post
                        .AllowAnyHeader();         //para receber o token
                    });
            });

            var app = builder.Build();

            //Criar o Banco de Dados e as Tabelas
            using (var scope = app.Services.CreateAsyncScope()) //CreateAsyncScope cria o banco de dados e tabelas ele consulta a classe de contexto identifica todas as tabelas que tem que criar e cria o banco e tabelas
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Inicializa o CORS
            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}