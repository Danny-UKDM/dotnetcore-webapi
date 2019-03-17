using System.Net.Http;
using Amazon.S3;
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

            AmazonS3Client = new AmazonS3Client(new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            });

            ImageBucketName = Configuration.GetSection("S3Buckets")["Images"];
        }

        private IConfiguration Configuration => Server.Host.Services.GetRequiredService<IConfiguration>();
        public ISessionFactory SessionFactory { get; }
        public AmazonS3Client AmazonS3Client { get; }
        public string ImageBucketName { get; }
    }
}
