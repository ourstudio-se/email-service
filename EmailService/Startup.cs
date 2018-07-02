using System;
using EmailService.Properties;
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
            EmailProperties emailProperties = EmailProperties.CreateInstance();
            
            services.AddSingleton(emailProperties);
            services.AddScoped<IHtmlGeneratorService, HtmlGeneratorService>();

            AddEmailService(emailProperties, services);
                
            services.AddMvc();
        }

        private void AddEmailService(EmailProperties emailProperties, IServiceCollection services)
        {
            bool isSendgrid = emailProperties.EmailService.Equals("sendgrid");

            if (isSendgrid)
            {
                services.AddScoped<IEmailServiceDefinition, SendGridEmailServiceDefinition>();
                services.AddScoped<IEmailService, HttpPostEmailService>();
            }
            else
            {
                throw new ArgumentException("A valid email service was not provided.");
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
