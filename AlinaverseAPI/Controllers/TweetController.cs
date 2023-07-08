using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlinaverseAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TweetController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTweets()
    {
        return Ok(new {Status = "Ok"});
    }
}