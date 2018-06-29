using EmailService.Service;
using EmailService.Service.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITemplatingService>(new FilesystemTemplatingService());
            
            services.AddScoped<IEmailServiceConfiguration, SendGridEmailServiceConfiguration>();
            services.AddScoped<IEmailService, SendGridEmailService>();

            services.AddSingleton(EmailConfiguration.CreateInstance());
                
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
