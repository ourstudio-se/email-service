using EmailService.Configuration;
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
            services.AddScoped<IEmailServiceConfiguration, SendGridEmailServiceConfiguration>();
            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<IHtmlGeneratorService, HtmlGeneratorService>();

            services.AddSingleton(EmailConfiguration.CreateInstance());
                
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
