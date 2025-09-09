
using Microsoft.AspNetCore.Mvc;

namespace PWC.Challenge.Api.Controllers
{
    [Route("dummy")]
    public class DummyController:ControllerBase
    {
    [HttpGet("foo")]
        public  async Task<IActionResult> GetDummy()
        {
            return Ok();
        }
    }
}
