using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Controllers.Services;
using WebApi.Tools;

namespace WebApi
{
    public class Startup
    {
        private DbFactory _dbFactory;

        public IConfiguration Configuration { get; }

        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _logger = logger;
            ApplicationLogging.LoggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Starting ConfigureServices.");

            services.AddSingleton<IEventRepository, EventRepository>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
        }

        private void OnApplicationStarted()
        {
            _logger.LogInformation("Starting OnApplicationStarted.");

            _dbFactory = new DbFactory(database: "content");
            _dbFactory.InitDatabase();
        }

        private void OnApplicationStopped()
        {
            _logger.LogInformation("Starting OnApplicationStopped.");

            _dbFactory.Dispose();
        }
    }
}
