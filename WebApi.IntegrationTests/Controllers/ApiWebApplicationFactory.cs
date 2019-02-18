using System.Net.Http;
using Badger.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace WebApi.IntegrationTests.Controllers
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public HttpClient Client { get; }

        public ApiWebApplicationFactory()
        {
            Client = CreateClient(); // Ensures Server is started before any tests execute
            SessionFactory = Badger.Data.SessionFactory.With(config =>
                config.WithConnectionString(Configuration.GetConnectionString("Content"))
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }

        private IConfiguration Configuration => Server.Host.Services.GetRequiredService<IConfiguration>();
        public ISessionFactory SessionFactory { get; }
    }
}
