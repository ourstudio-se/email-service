using System;
using EmailService.Configurations;
using EmailService.Controllers;
using EmailService.Database;
using EmailService.Service;
using EmailService.Service.Implementations;
using EmailService.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            EmailConfiguration emailConfiguration = ConfigurationUtility.CreateInstance<EmailConfiguration>(
                EmailConfiguration.CONFIGURATION_PATH);
            
            ServiceConfiguration serviceConfiguration = ConfigurationUtility.CreateInstance<ServiceConfiguration>(
                ServiceConfiguration.CONFIGURATION_PATH);

            ParseServiceConfigurationEnums(serviceConfiguration);
            
            services.AddSingleton(emailConfiguration);
            services.AddSingleton(serviceConfiguration);

            services.AddTransient<EmailPreviewController, EmailPreviewController>();

            services.AddLogging(builder => builder
                .AddConsole()
                .AddDebug());
            
            services.AddScoped<IHtmlGeneratorService, HtmlGeneratorService>();
            services.AddScoped<IEmailLoggingService, DefaultEmailLogginService>();
            
            AddEmailService(serviceConfiguration, services);

            bool isDatabaseLogging =
                serviceConfiguration.SelectedLoggingType.Equals(ServiceConfiguration.LoggingType.DATABASE);

            if (isDatabaseLogging)
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(serviceConfiguration.LoggingDatabaseConnectionString));
            }
            else
            {
                services.AddDbContext<DataContext>();
            }
                
            services.AddMvc();
        }

        private void ParseServiceConfigurationEnums(ServiceConfiguration serviceConfiguration)
        {
            serviceConfiguration.SelectedLoggingType =
                ServiceConfigurationUtility.ParseLoggingType(serviceConfiguration.LoggingTypeString);
            
            serviceConfiguration.SelectedEmailService =
                ServiceConfigurationUtility.ParseEmailService(serviceConfiguration.EmailServiceString);

            bool hasInvalidLoggingType =
                serviceConfiguration.SelectedLoggingType == ServiceConfiguration.LoggingType.INVALID;

            if (hasInvalidLoggingType)
            {
                throw new Exception("Invalid logging type specified in serviceConfiguration.");
            }

            bool hasInvalidEmailService =
                serviceConfiguration.SelectedEmailService == ServiceConfiguration.EmailService.INVALID;

            if (hasInvalidEmailService)
            {
                throw new Exception("Invalid email service specified in serviceConfiguration.");
            }
        }

        private void AddEmailService(ServiceConfiguration serviceConfiguration, IServiceCollection services)
        {
            bool isSendgrid = serviceConfiguration.SelectedEmailService.Equals(ServiceConfiguration.EmailService.SENDGRID);

            if (isSendgrid)
            {
                services.AddScoped<IEmailServiceDefinition, SendGridEmailServiceDefinition>();
                services.AddScoped<IEmailService, HttpPostEmailService>();
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            DataContext dataContext = serviceProvider.GetService<DataContext>();

            bool shouldMigrateDbOnStartup = Configuration.GetValue<bool>("MigrateDbOnStartup");

            if (shouldMigrateDbOnStartup)
            {
                try
                {
                    dataContext.Database.Migrate();
                }
                catch (Exception e)
                {
                }
            }
            
            app.UseMvc();
        }
    }
}
