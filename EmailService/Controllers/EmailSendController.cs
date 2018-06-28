using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/email/send")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // POST api/email/send
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
