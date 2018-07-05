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
	[Route("api/email")]
	[ApiController]
	public class EmailController : Controller
	{
		private readonly IHtmlGeneratorService _htmlGeneratorService;
		private readonly EmailConfiguration _emailConfiguration;
		private readonly IEmailLoggingService _loggingService;

		public EmailController(IHtmlGeneratorService htmlGeneratorService, EmailConfiguration emailConfiguration,
			IEmailLoggingService emailLoggingService)
		{
			_htmlGeneratorService = htmlGeneratorService;
			_emailConfiguration = emailConfiguration;
			_loggingService = emailLoggingService;
		}
		
		// GET api/email
		
		/// <summary>
		/// Health check for the email service.
		/// </summary>
		/// <response code="200">The service is functional.</response>
		[HttpGet]
		[Route("", Name = "HealthCheck")]
		public async Task<IActionResult> Get()
		{
			return new OkResult();
		}
		
		// GET api/email/{id}

		/// <summary>
		/// Return information about an email that has been sent by the service. This endpoint is only usable
		/// if you are storing email logs with the database method.
		/// </summary>
		/// <param name="id">The id of the email log entry.</param>
		/// <param name="receiversToTest">A JSON array containing receivers to test if they were receivers of the
		/// email. Since the email receivers are hashed in the logs we can only know if someone was included in the email
		/// or not, but not get a list of who were included. The output parameter "MatchedReceiversToTest" reflects
		/// if the match was true or not.</param>
		/// <response code="200">If the request was valid.</response>
		/// <response code="400">If the request was bad.</response>
		[HttpGet]
		[Route("{id}", Name = "GetEmail")]
		[ProducesResponseType(typeof(SentEmailResponse), 200)]
		public async Task<IActionResult> Get([FromRoute] string id,
			[FromQuery(Name = "receiversToTest")] string receiversToTest = null)
		{
			bool isValidId = Guid.TryParse(id, out Guid guid);

			if (!isValidId)
			{
				return new BadRequestObjectResult("The id specified had an invalid format.");
			}

			LogEntry logEntry = await _loggingService.GetAsync(guid);

			bool noLogEntryFound = logEntry == null;

			if (noLogEntryFound)
			{
				return new BadRequestObjectResult("No logs found for the id provided.");
			}
			
			JObject content = JObject.Parse(logEntry.Content);
			JObject personalContent = JObject.Parse(logEntry.PersonalContent);
			
			EmailSendRequest request = new EmailSendRequest()
			{
				Content = content,
				PersonalContent = personalContent,
				Template = logEntry.Template,
				To = new string[0]
			};

			OkObjectResult actionResult = await Post(request) as OkObjectResult;
			EmailPreviewResponse previewResponse = (EmailPreviewResponse) actionResult.Value;

			string[] receiversToTestArray = RestUtility.GetArrayQueryParam(receiversToTest);

			bool hasReceiversToTestArray = receiversToTestArray != null && receiversToTestArray.Length > 0;
			bool isReceiversMatch;

			if (hasReceiversToTestArray)
			{
				isReceiversMatch = TestReceiversMatch(receiversToTestArray, logEntry);
			}
			else
			{
				isReceiversMatch = false;
			}

			SentEmailResponse response = SentEmailResponseFactory.Create(logEntry, previewResponse, isReceiversMatch);
			return new OkObjectResult(response);
		}

		// POST api/email
		
		/// <summary>
		/// Preview what an email will look like, without actually sending the email.
		/// </summary>
		/// <param name="request">The email payload, which should be the same fromat as the payload to the /api/email/send
		/// endpoint.</param>
		/// <response code="200">The request is valid.</response>
		/// <response code="400">The request is invalid.</response>
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

		private bool TestReceiversMatch(string[] receiversToTestArray, LogEntry logEntry)
		{
			string[] logEntryTo = ArrayUtility.GetArrayFromCommaSeparatedString(logEntry.To);
			bool isSameSize = logEntryTo.Length == receiversToTestArray.Length;

			if (!isSameSize)
			{
				return false;
			}

			foreach (string receiver in receiversToTestArray)
			{
				string hashedReceiver = HashUtility.GetStringHash(receiver);
				bool isReceiverInLogEntry = logEntryTo.Contains(hashedReceiver);

				if (!isReceiverInLogEntry)
				{
					return false;
				}
			}

			return true;
		}
	}
}