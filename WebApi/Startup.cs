using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Tools;

namespace WebApi
{
    public class Startup
    {
        private DbFactory _dbFactory;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            lifetime.ApplicationStarted.Register(OnApplicationStarted);
            lifetime.ApplicationStopped.Register(OnApplicationStopped);
   
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void OnApplicationStarted()
        {
            _dbFactory = new DbFactory("content");
            _dbFactory.InitDatabase();
        }

        private void OnApplicationStopped()
        {
            _dbFactory.Dispose();
        }
    }
}
