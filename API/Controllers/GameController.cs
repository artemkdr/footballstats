using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers;

public class GameModel
{
    public string? Status { get; set; }
    public int Team1 { get; set; }    
    public int Team2 { get; set; }    
    public int Goals1 { get; set; }    
    public int Goals2 { get; set; }    
    
}

[ApiController]
public class GameController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;
    
    public GameController(IConfiguration configuration)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
    }

    [Route("game/{id}")]    
    [HttpGet]
    public IActionResult GetGame(int id)
    {
        return RequestHelpers.Success(RequestHelpers.ToDict("id", id));
    }

    [Route("game")]
    [HttpPost]
    public IActionResult CreateGame(GameModel data)
    {
        if (data != null && data.Status != null) {
            return RequestHelpers.Success(RequestHelpers.ToDict("status", data.Status));
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "status is missing"));
    }

    [Route("game/{id}")]
    [HttpPost]
    public IActionResult UpdateGame(GameModel data)
    {
        if (data != null && data.Status != null) {
            return RequestHelpers.Success(RequestHelpers.ToDict("status", data.Status));
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "status is missing"));
    }
}