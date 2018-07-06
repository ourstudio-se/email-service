using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.Configurations;
using EmailService.Dtos.Requests;
using EmailService.Dtos.Requests.Factories;
using EmailService.Dtos.Responses;
using EmailService.Dtos.Responses.Factories;
using EmailService.Models;
using EmailService.Service;
using EmailService.Utils;
using EmailService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EmailService.Controllers
{
	[Route("api/email/preview")]
	[ApiController]
	public class EmailPreviewController : Controller
	{
		private readonly IHtmlGeneratorService _htmlGeneratorService;
		private readonly EmailConfiguration _emailConfiguration;

		public EmailPreviewController(IHtmlGeneratorService htmlGeneratorService, EmailConfiguration emailConfiguration)
		{
			_htmlGeneratorService = htmlGeneratorService;
			_emailConfiguration = emailConfiguration;
		}
		
		// GET api/email/preview
		
		/// <summary>
		/// Preview what an email will look like, without actually sending the email.
		/// </summary>
		/// <param name="request">The email payload, which should be the same fromat as the payload to the /api/email/send
		/// endpoint.</param>
		/// <response code="200">The request is valid.</response>
		/// <response code="400">The request is invalid.</response>
        [HttpGet]
        [Route("", Name = "PreviewEmailGet")]
        [Produces("text/html")]
        public async Task<IActionResult> Get([FromQuery] EmailSendRequest request)
        {
	        OkObjectResult actionResult = await Post(request) as OkObjectResult;
	        EmailPreviewResponse previewResponse = (EmailPreviewResponse) actionResult.Value;

	        return new ContentResult()
	        {
		        Content = previewResponse.Preview,
		        ContentType = "text/html",
		        StatusCode = 200
	        };
        }
		
		// POST api/email/preview
		
		/// <summary>
		/// Preview what an email will look like, without actually sending the email.
		/// </summary>
		/// <param name="request">The email payload, which should be the same fromat as the payload to the /api/email/send
		/// endpoint.</param>
		/// <response code="200">The request is valid.</response>
		/// <response code="400">The request is invalid.</response>
        [HttpPost]
        [Route("", Name = "PreviewEmailPost")]
        [ProducesResponseType(typeof(EmailPreviewResponse), 200)]
        public async Task<IActionResult> Post([FromBody] EmailSendRequest request)
        {
	        bool isValidEmailRequest = EmailSendRequestFactory.IsValidEmailRequest(request);

            if (!isValidEmailRequest)
            {
                return new BadRequestObjectResult("Invalid email payload.");
            }

	        bool hasNoContent = request.Content == null;

	        if (hasNoContent)
	        {
		        request.Content = new JObject();
	        }

	        bool hasNoPersonalContent = request.PersonalContent == null;

	        if (hasNoPersonalContent)
	        {
		        request.PersonalContent = new JObject();
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

	        Template template = TemplateUtility.GetTemplateByName(_emailConfiguration, request.Template);

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