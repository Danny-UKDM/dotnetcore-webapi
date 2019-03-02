using System.IO.Pipes;
using Badger.Data;
using DatabaseInitialiser;
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
        private Initialiser _initialiser;

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

            services.AddTransient<IImageWriter, ImageWriter>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(x =>
                {
                    x.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info {Title = "Content API", Version = "v1"});
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

            _initialiser = new Initialiser(Configuration.GetConnectionString("Content"));
            _initialiser.Init();
        }

        private void OnApplicationStopped()
        {
            _logger.LogInformation("Tearing down database.");

            _initialiser.Dispose();
        }
    }
}
