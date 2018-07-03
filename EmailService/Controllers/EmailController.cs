using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.Dtos.Requests;
using EmailService.Dtos.Requests.Factories;
using EmailService.Dtos.Responses;
using EmailService.Dtos.Responses.Factories;
using EmailService.Models;
using EmailService.Properties;
using EmailService.Service;
using EmailService.Utils;
using EmailService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EmailService.Controllers
{
	[Route("api/email")]
	[ApiController]
	public class EmailController : Controller
	{
		private readonly IHtmlGeneratorService _htmlGeneratorService;
		private readonly EmailProperties _emailProperties;

		public EmailController(IHtmlGeneratorService htmlGeneratorService, EmailProperties emailProperties)
		{
			_htmlGeneratorService = htmlGeneratorService;
			_emailProperties = emailProperties;
		}
		
		// GET api/email
		[HttpGet]
		[Route("", Name = "HealthCheck")]
		public async Task<IActionResult> Get()
		{
			return new OkResult();
		}
		
		// GET api/email/{id}
		[HttpGet]
		[Route("", Name = "GetEmail")]
		public async Task<IActionResult> Get([FromRoute] string id)
		{
			return new OkResult();
		}
		
		// POST api/email
        [HttpPost]
        [Route("", Name = "PreviewEmail")]
        [ProducesResponseType(typeof(EmailPreviewResponse), 200)]
        public async Task<IActionResult> Post([FromBody] EmailSendRequest request)
        {
	        bool isValidEmailRequest = EmailSendRequestFactory.IsValidEmailRequest(request);

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

	        Template template = TemplateUtility.GetTemplateByName(_emailProperties, request.Template);

            bool isInvalidTemplate = template == null;

            if (isInvalidTemplate)
            {
                return new BadRequestObjectResult($"A template with the name {request.Template} does not exist.");
            }

	        JObject fullContent = JsonUtility.GetMergedJson(request.Content, request.PersonalContent);

            EmailViewModel emailViewModel = new EmailViewModel() {TemplateName = template.Name, Content = fullContent};
            string rawHtml = await _htmlGeneratorService.GetRawHtmlAsync("Email/Index", emailViewModel);

            bool hasNoRawHtml = rawHtml == null;

            if (hasNoRawHtml)
            {
                return new BadRequestObjectResult("Internal error.");
            }
            
            Email email = new Email(toAddresses, template.Subject, ContentType.TEXT_HTML, rawHtml);
	        EmailPreviewResponse response = EmailPreviewResponseFactory.Create(email);
	        
	        return new OkObjectResult(response);
        }
	}
}