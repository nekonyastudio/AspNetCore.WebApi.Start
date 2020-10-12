using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nekonya.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> SayHello()
        {
            return Task.FromResult<IActionResult>(Ok(new { msg = "Hello" }));
        }

        [Authorize]
        [HttpGet("Oidc")]
        public Task<IActionResult> HelloOidc()
        {
            return Task.FromResult<IActionResult>(Ok(new { msg = "Hello", subjectId = this.User.Identity.GetSubjectId() }));
        }
    }
}
