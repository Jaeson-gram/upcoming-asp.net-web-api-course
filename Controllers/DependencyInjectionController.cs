using Microsoft.AspNetCore.Mvc;
using WebAPI2.Logs;

namespace WebAPI2.Controllers;

/*
 * 1. Tightly/strongly coupled mech


[Route("api/[controller]")]
[ApiController]
public class DependencyInjectionController : ControllerBase
{
    private readonly ILog _logger;

    public DependencyInjectionController()
    {
        _logger = new LogToFile();
    }
    
    [HttpGet]
    public ActionResult Index()
    {
        _logger.Log("Index");
        
        return Ok();
    }
    
}
 */
 
 //2. Loosely coupled mech
 [Route("api/[controller]")]
 [ApiController]
 public class DependencyInjectionController : ControllerBase
 {
     private readonly ILog _log;

     public DependencyInjectionController(ILog log)
     {
         _log = log;
     }

     [HttpGet("basic")]
     public ActionResult Index()
     {
         _log.Log("Index");
         
         return Ok();
     }
 }