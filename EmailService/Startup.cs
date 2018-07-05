using System;
using EmailService.Database;
using EmailService.Properties;
using EmailService.Service;
using EmailService.Service.Implementations;
using EmailService.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            services.AddScoped<IEmailLoggingService, DefaultEmailLogginService>();

            ServiceProperties serviceProperties = CreateServiceProperties();

            services.AddSingleton(serviceProperties);
            AddEmailService(serviceProperties, emailProperties, services);

            bool isDatabaseLogging =
                serviceProperties.SelectedLoggingType.Equals(ServiceProperties.LoggingType.DATABASE);

            if (isDatabaseLogging)
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(serviceProperties.LoggingDatabaseConnectionString));
            }
                
            services.AddMvc();
        }

        private ServiceProperties CreateServiceProperties()
        {
            string selectedEmailService = Configuration.GetValue<string>("EmailService");
            string emailServiceUrl = Configuration.GetValue<string>("EmailServiceUrl");
            string emailServiceApiKey = Configuration.GetValue<string>("EmailServiceApiKey");
            
            string selectedLoggingType = Configuration.GetValue<string>("LoggingType");
            string loggingApiUrl = Configuration.GetValue<string>("LoggingApiUrl");
            string loggingDatabaseConnectionString = Configuration.GetValue<string>("LoggingDatabaseConnectionString");

            ServiceProperties.EmailService? emailService =
                ServicePropertiesUtility.ParseEmailService(selectedEmailService);

            bool hasNoEmailService = emailService == null;

            if (hasNoEmailService)
            {
                throw new ArgumentException("A valid email service was not provided.");
            }
            
            ServiceProperties.LoggingType? loggingType = ServicePropertiesUtility.ParseLoggingType(selectedLoggingType);
            bool hasNoLoggingType = loggingType == null;

            if (hasNoLoggingType)
            {
                throw new ArgumentException("A valid logging type was not provided.");
            }

            return new ServiceProperties()
            {
                SelectedEmailService = (ServiceProperties.EmailService) emailService,
                EmailServiceApiKey = emailServiceApiKey,
                EmailServiceUrl = emailServiceUrl,
                
                SelectedLoggingType = (ServiceProperties.LoggingType) loggingType,
                LoggingApiUrl = loggingApiUrl,
                LoggingDatabaseConnectionString = loggingDatabaseConnectionString
            };
        }

        private void AddEmailService(ServiceProperties serviceProperties, EmailProperties emailProperties,
            IServiceCollection services)
        {
            bool isSendgrid = serviceProperties.SelectedEmailService.Equals(ServiceProperties.EmailService.SENDGRID);

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
