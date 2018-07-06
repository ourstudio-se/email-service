using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
	[Route("api/email")]
	[ApiController]
	public class EmailController : Controller
	{
		// GET api/email
		
		/// <summary>
		/// Health check for the email service.
		/// </summary>
		/// <response code="200">The service is functional.</response>
		[HttpGet]
		[Route("", Name = "HealthCheck")]
		public IActionResult Get()
		{
			return new OkResult();
		}
	}
}