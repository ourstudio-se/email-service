using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
	[Route("api/email")]
	[ApiController]
	public class EmailController : Controller
	{
		// GET api/email
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return new OkResult();
		}
	}
}