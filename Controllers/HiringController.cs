using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI2.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors(PolicyName = "ForHire")] 

// so all hiring related code will be here,
// enabling cors so only origins allowed in the named policy will have access here. this will override the middleware policy

public class HiringController
{
    // bla bla bla
}