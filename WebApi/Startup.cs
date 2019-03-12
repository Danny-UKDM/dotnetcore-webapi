using Badger.Data;
using DatabaseInitialiser;
using LocalstackInitialiser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebApi.Data.Writers;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly ILogger<Startup> _logger;
        private Initialiser _dbInitialiser;
        private S3Initialiser _s3Initialiser;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Starting ConfigureServices.");

            services.AddSingleton(SessionFactory.With(config =>
                config.WithConnectionString(Configuration.GetConnectionString("Content"))
                      .WithProviderFactory(NpgsqlFactory.Instance)));

            services.AddTransient<IImageRepository, ImageRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(x =>
                {
                    x.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Content API", Version = "v1" });
                });

            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime, IHostingEnvironment env)
        {
            _logger.LogInformation("Starting Configure.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            lifetime.ApplicationStarted.Register(OnApplicationStarted);
            lifetime.ApplicationStopping.Register(OnApplicationStopped);

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Content API v1");
            });
        }

        private void OnApplicationStarted()
        {
            _logger.LogInformation("Creating database.");
            _dbInitialiser = new Initialiser(Configuration.GetConnectionString("Content"));
            _dbInitialiser.Init();

            _logger.LogInformation("Creating s3 bucket.");
            _s3Initialiser = new S3Initialiser(Configuration.GetSection("S3Buckets")["Images"]);
            _s3Initialiser.Init();
        }

        private void OnApplicationStopped()
        {
            _logger.LogInformation("Tearing down database.");
            _dbInitialiser.Dispose();

            _logger.LogInformation("Destroying s3 bucket.");
            _s3Initialiser.Dispose();
        }
    }
}
