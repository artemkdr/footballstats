using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers;

public class TeamModel
{
    public string? Name { get; set; }
    public string[]? Players { get; set; }    
}

[ApiController]
public class TeamController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;
    
    public TeamController(IConfiguration configuration)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
    }

    [Route("team/{id}")]    
    [HttpGet]
    public IActionResult GetTeam(int id)
    {
        return RequestHelpers.Success(RequestHelpers.ToDict("id", id));
    }

    [Route("team")]
    [HttpPost]
    public IActionResult CreateTeam(TeamModel data)
    {
        if (data != null && data.Name != null) {
            return RequestHelpers.Success(RequestHelpers.ToDict("name", data.Name));
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "name is missing"));
    }

    [Route("team/{id}")]
    [HttpPost]
    public IActionResult UpdateTeam(TeamModel data)
    {
        if (data != null && data.Name != null) {
            return RequestHelpers.Success(RequestHelpers.ToDict("name", data.Name));
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "name is missing"));
    }
}