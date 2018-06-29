using System;
using System.Net.Mail;
using EmailService.Dtos.Requests;
using EmailService.Models;
using EmailService.Service;
using EmailService.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/email/send")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITemplatingService _templatingService;
        private readonly IEmailService _emailService;
        private readonly IEmailConfiguration _emailConfiguration;
        
        public ValuesController(ITemplatingService templatingService, IEmailService emailService, IEmailConfiguration emailConfiguration)
        {
            _templatingService = templatingService;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration;
        }
        
        // POST api/email/send
        [HttpPost]
        [Route("", Name = "SendEmail")]
        public IActionResult Post([FromBody] EmailSendRequest request)
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

            Template template = _templatingService.GetTemplate(request.Template);

            bool isInvalidTemplate = template == null;

            if (isInvalidTemplate)
            {
                return new BadRequestObjectResult($"A template with the name {request.Template} does not exist.");
            }

            bool isInvalidContent = !TemplateContentValidator.IsValidContentForTemplate(template, request.Content);

            if (isInvalidContent)
            {
                return new BadRequestObjectResult("The content provided does not match the arguments needed for the template.");
            }

            string rawHtml = HtmlGenerator.GenerateRawHtml(template, request.Content);
            
            Email email = new Email(toAddress, "TODO", ContentType.TEXT_HTML, rawHtml);
            _emailService.SendEmail(_emailConfiguration, email);
            
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
