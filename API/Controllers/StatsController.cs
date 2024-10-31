using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class StatsDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int Games { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int GF { get; set; }

    public int GA { get; set; }
}

public class StatsRivalsDTO
{
    public int Team1 { get; set; }

    public int Team2 { get; set; }

    public int Wins1 { get; set; }

    public int Wins2 { get; set; }

    public int Draws { get; set; }
}


[ApiController]
public class StatsController : BaseController
{

    private readonly IStatsService _statsService; // Inject the service

    public StatsController(
        IConfiguration configuration, 
        BaseDBContext<User> userContext, 
        BaseDBContext<Team> teamContext, 
        BaseDBContext<Game> gameContext,
        IStatsService statsService) 
        : base(configuration, userContext, teamContext, gameContext)
    {
        _statsService = statsService;
    }

    [Route("stats")]    
    [HttpGet]
    public async Task<IActionResult> GetStats(int? team = null, string? sort = "default")
    {        
        var stats = await _statsService.GetStatsAsync(team, sort);    
        return Ok(stats);        
    }

    [Route("statsrivals")]    
    [HttpGet]
    public async Task<IActionResult> GetStatsRivals(int team1, int team2)
    {        
        var stats = await _statsService.GetStatsRivalsAsync(team1, team2);
        return Ok(stats);        
    }

    
}