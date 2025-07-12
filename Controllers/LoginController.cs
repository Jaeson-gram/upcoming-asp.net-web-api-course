using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI2.Models;

namespace WebAPI2.Controllers;

[Route("api/[controller]")]
[ApiController]
// [EnableCors(PolicyName = "LocalHost")]
// [Authorize(AuthenticationSchemes = "LoginForLocal", Roles = "SuperAdmin,Admin")]
public class LoginController : Controller
{
    private readonly IConfiguration _configuration;

    public LoginController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpPost]
    public ActionResult<LoginResponseDTO> Login(LoginDTO loginDetails)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest("please provide username and password");
        }
        
        var loginResponse = new LoginResponseDTO{Username = loginDetails.Username};

        var audience = string.Empty;
        var issuer = string.Empty;

        byte[] key = null;

        switch (loginDetails.Policy)
        {
            case "Local":
                issuer = _configuration.GetValue<string>("jwtLocalIssuer"); // ?? "LocalAuth - issuer";
                audience = _configuration.GetValue<string>("jwtLocalAudience"); // ?? "Local";
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("jwtSecretForLocal")); // ?? "this is the most ultimate secret: every secret will be revealed - LOCAL");
                break;
            case "Hire":
                issuer = _configuration.GetValue<string>("jwtHireIssuer");
                audience = _configuration.GetValue<string>("jwtHireAudience");
            
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("jwtSecretForHireWeb"));
                break;
            case "General":
                issuer = _configuration.GetValue<string>("jwtIssuer");
                audience = _configuration.GetValue<string>("jwtAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("jwtSecret"));
                break;
        }

        // if the credentials are accepted then we use the JwtSecurityTokenHandler() to create a token...
        // which requires the SecurityTokenDescriptor() to arrange for claims, singing credentials (key and tokenisation algorithm), and any other available option of choice
        // these satisfy the requirements for a jwt token -> payload (provided by claims), head - which holds the type of auth and algorithm used - and key, both provided by the SigningCredentials() 
        // then simply return the token
        
        // if (loginDetails.Username == "jey" && loginDetails.Password == "palourmamaritasabrina") // -> merged into pattern
        if (loginDetails is { Username: "jey", Password: "palourmamaritasabrina" })
        {
            // var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("jwtSecret"));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                
                // Subject = new ClaimsIdentity(new Claim[]
                Subject = new ClaimsIdentity([
                    // username
                    new Claim(ClaimTypes.Name, loginDetails.Username),
                    // role 
                    new Claim(ClaimTypes.Role, "SuperAdmin")
                    
                ]), 
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            loginResponse.Token = tokenHandler.WriteToken(token);
        }
        else
        {
            return Unauthorized();
        }
        
        return Ok(loginResponse);
    }
}
