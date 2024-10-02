using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;
    
    public HealthController(IConfiguration configuration)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
    }

    [Route("health")]    
    [HttpGet]
    public IActionResult Get()
    {
        return RequestHelpers.Success();
    }
}