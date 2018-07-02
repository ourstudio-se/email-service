﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.Dtos.Requests;
using EmailService.Models;
using EmailService.Properties;
using EmailService.Service;
using EmailService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/email/send")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly EmailProperties _emailProperties;
        private readonly IEmailService _emailService;
        private readonly IEmailServiceDefinition _emailServiceDefinition;
        private readonly IHtmlGeneratorService _htmlGeneratorService;
        
        public ValuesController(EmailProperties emailProperties, IEmailService emailService, IEmailServiceDefinition emailServiceDefinition, IHtmlGeneratorService htmlGeneratorService)
        {
            _emailProperties = emailProperties;
            _emailService = emailService;
            _emailServiceDefinition = emailServiceDefinition;
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

            Template template = GetEmailTemplateByName(request.Template);

            bool isInvalidTemplate = template == null;

            if (isInvalidTemplate)
            {
                return new BadRequestObjectResult($"A template with the name {request.Template} does not exist.");
            }

            EmailViewModel emailViewModel = new EmailViewModel() {TemplateName = template.Name, Content = request.Content};
            string rawHtml = await _htmlGeneratorService.GetRawHtmlAsync("Email/Index", emailViewModel);

            bool hasNoRawHtml = rawHtml == null;

            if (hasNoRawHtml)
            {
                return new BadRequestObjectResult("Internal error.");
            }
            
            Email email = new Email(toAddress, template.Subject, ContentType.TEXT_HTML, rawHtml);

            try
            {
                await _emailService.SendEmailAsync(_emailProperties, _emailServiceDefinition, email);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult("Failed to send email.");
            }
        }

        private Template GetEmailTemplateByName(string name)
        {
            return _emailProperties.Templates.FirstOrDefault(d => d.Name.ToLower().Equals(name.ToLower()));
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
