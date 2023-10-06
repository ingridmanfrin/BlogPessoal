using blogpessoal;
using blogpessoal.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using WebMotions.Fake.Authentication.JwtBearer;

namespace BlogPessoalTeste.Factory
{
    public class WebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryBlogPessoalTest");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                using var appContex = scope.ServiceProvider.GetService<AppDbContext>();

                try
                {
                    appContex.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            });
            builder.UseContentRoot(".");
            builder.UseTestServer().ConfigureTestServices(collection =>
            {
                collection.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                }
                ).AddFakeJwtBearer();
            });
            base.ConfigureWebHost(builder);
        }
    }
}
