using System;
using System.Threading.Tasks;
using System.Linq;
using EmailService.Dtos.Requests;
using EmailService.Dtos.Responses;
using EmailService.Dtos.Responses.Factories;
using EmailService.Models;
using EmailService.Service;
using EmailService.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EmailService.Controllers
{
	[Route("api/email/logs")]
	[ApiController]
	public class EmailLogController : Controller
	{
		private readonly IEmailLoggingService _loggingService;
		private readonly EmailPreviewController _emailPreviewController;
		
		public EmailLogController(IEmailLoggingService emailLoggingService, EmailPreviewController emailPreviewController)
		{
			_loggingService = emailLoggingService;
			_emailPreviewController = emailPreviewController;
		}
		
		// GET api/email/log/{id}

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
		[Route("{id}", Name = "GetEmailLog")]
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

			OkObjectResult actionResult = await _emailPreviewController.Post(request) as OkObjectResult;
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