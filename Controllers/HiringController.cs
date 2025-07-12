using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI2.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors(PolicyName = "ForHire")] 
[Authorize(AuthenticationSchemes = "LoginForHiring")] // the auth scheme here 

// cors ...
// so all hiring related code will be here,
// enabling cors so only origins allowed in the named policy 'ForHire' will have access here. this will override the middleware policy
// [EnableCors(PolicyName = "LocalHost")] -- you can also configure policy for individual endpoints, which overrides the controller policy


// jwt auth
// the auth scheme will ensure that only requests made with a header that contains the 'Hire' as audience will have access to these endpoints
// if a user logs in with 'Local' or any other policy name, they will not have access to these endpoints
public class HiringController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("the hiring endpoint works after verifying issuer and audience");
    }
    // bla bla bla
}