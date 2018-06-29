using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.Configuration;
using EmailService.Dtos.Requests;
using EmailService.Models;
using EmailService.Service;
using EmailService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/email/send")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IEmailService _emailService;
        private readonly IEmailServiceConfiguration _emailServiceConfiguration;
        private readonly IHtmlGeneratorService _htmlGeneratorService;
        
        public ValuesController(EmailConfiguration emailConfiguration, IEmailService emailService, IEmailServiceConfiguration emailServiceConfiguration, IHtmlGeneratorService htmlGeneratorService)
        {
            _emailConfiguration = emailConfiguration;
            _emailService = emailService;
            _emailServiceConfiguration = emailServiceConfiguration;
            _htmlGeneratorService = htmlGeneratorService;
        }
        
        // POST api/email/send
        [HttpPost]
        [Route("", Name = "SendEmail")]
        public async Task<IActionResult> Post([FromBody] EmailSendRequest request)
        {
            bool isValidEmailRequest = IsValidEmailRequest(request);

            if (!isValidEmailRequest)
            {
                return new BadRequestObjectResult("Invalid email payload.");
            }

            MailAddress toAddress;

            try
            {
                toAddress = new MailAddress(request.To);
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult($"Invalid format of recipient email {request.To}.");
            }

            Template template = _emailConfiguration.Templates.FirstOrDefault(d => d.Name.Equals(request.Template));

            bool isInvalidTemplate = template == null;

            if (isInvalidTemplate)
            {
                return new BadRequestObjectResult($"A template with the name {request.Template} does not exist.");
            }

            EmailViewModel emailViewModel = new EmailViewModel() {TemplateName = template.Name, Content = request.Content};
            string rawHtml = await _htmlGeneratorService.GetRawHtmlAsync("~/View/Email/Index.cshtml", emailViewModel);
            
            Email email = new Email(toAddress, template.Subject, ContentType.TEXT_HTML, rawHtml);
            _emailService.SendEmail(_emailConfiguration, _emailServiceConfiguration, email);
            
            return new OkResult();
        }

        private bool IsValidEmailRequest(EmailSendRequest request)
        {
            bool hasTo = !string.IsNullOrWhiteSpace(request.To);
            bool hasTemplate = !string.IsNullOrWhiteSpace(request.Template);
            bool hasContent = request.Content != null;

            return hasTo && hasTemplate && hasContent;
        }
    }
}
