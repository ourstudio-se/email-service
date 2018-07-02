using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.Dtos.Requests;
using EmailService.Models;
using EmailService.Properties;
using EmailService.Service;
using EmailService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EmailService.Controllers
{
    [Route("api/email/send")]
    [ApiController]
    public class EmailSendController : ControllerBase
    {
        private readonly EmailProperties _emailProperties;
        private readonly IEmailService _emailService;
        private readonly IEmailServiceDefinition _emailServiceDefinition;
        private readonly IHtmlGeneratorService _htmlGeneratorService;
        private readonly IEmailLoggingService _emailLoggingService;
        
        public EmailSendController(EmailProperties emailProperties, IEmailService emailService,
            IEmailServiceDefinition emailServiceDefinition, IHtmlGeneratorService htmlGeneratorService,
            IEmailLoggingService emailLoggingService)
        {
            _emailProperties = emailProperties;
            _emailService = emailService;
            _emailServiceDefinition = emailServiceDefinition;
            _htmlGeneratorService = htmlGeneratorService;
            _emailLoggingService = emailLoggingService;
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

            MailAddress[] toAddresses;

            try
            {
                toAddresses = request.To.Select(t => new MailAddress(t)).ToArray();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult($"Invalid format of recipient email {request.To}.");
            }

            Template template = GetEmailTemplateByName(request.Template);

            bool isInvalidTemplate = template == null;

            if (isInvalidTemplate)
            {
                return new BadRequestObjectResult($"A template with the name {request.Template} does not exist.");
            }

            JObject fullContent = CreateFullContent(request.Content, request.PersonalContent);

            EmailViewModel emailViewModel = new EmailViewModel() {TemplateName = template.Name, Content = fullContent};
            string rawHtml = await _htmlGeneratorService.GetRawHtmlAsync("Email/Index", emailViewModel);

            bool hasNoRawHtml = rawHtml == null;

            if (hasNoRawHtml)
            {
                return new BadRequestObjectResult("Internal error.");
            }
            
            Email email = new Email(toAddresses, template.Subject, ContentType.TEXT_HTML, rawHtml);

            try
            {
                string emailServiceId = await _emailService.SendEmailAsync(_emailProperties, _emailServiceDefinition,
                    email);

                string[] receiverEmails = email.To.Select(t => t.ToString()).ToArray();
                
                _emailLoggingService.Log(emailServiceId, receiverEmails, template.Name, request.PersonalContent,
                    request.Content);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult("Failed to send email.");
            }
        }

        private JObject CreateFullContent(JObject content, JObject personalContent)
        {
            JObject newObject = new JObject();
            
            newObject.Merge(content,
                new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
            
            newObject.Merge(personalContent,
                new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
            
            return content;
        }

        private Template GetEmailTemplateByName(string name)
        {
            return _emailProperties.Templates.FirstOrDefault(d => d.Name.ToLower().Equals(name.ToLower()));
        }

        private bool IsValidEmailRequest(EmailSendRequest request)
        {
            bool hasTo = request.To != null && request.To.Length > 0;
            bool hasTemplate = !string.IsNullOrWhiteSpace(request.Template);
            bool hasContent = request.Content != null;

            return hasTo && hasTemplate && hasContent;
        }
    }
}
