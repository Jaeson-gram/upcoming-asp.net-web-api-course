using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI2.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors(PolicyName = "ForHire")] 

// so all hiring related code will be here,
// enabling cors so only origins allowed in the named policy 'ForHire' will have access here. this will override the middleware policy
// [EnableCors(PolicyName = "LocalHost")] -- you can also configure policy for individual endpoints, which overrides the controller policy

public class HiringController
{
    // bla bla bla
}