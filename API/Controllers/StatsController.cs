using System.Net;
using System.Text.Json;
using API.Data;
using API.Models;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;

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


[ApiController]
public class StatsController : BaseController
{
    public StatsController(IConfiguration configuration, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext, BaseDBContext<Game> gameContext) 
        : base(configuration, userContext, teamContext, gameContext)
    {
    }

    [Route("stats")]    
    [HttpGet]
    public IActionResult GetStats()
    {
        try {
            var stats = _gameContext.Items
                .Where(x => x.Status == GameStatus.Completed)
                .GroupBy(x => new { x.Team1 })
                .Select(x => new StatsDTO
                {
                    Id = x.Key.Team1,                    
                    Games = x.Count(),
                    Wins = x.Count(x => x.Goals1 > x.Goals2),
                    Losses = x.Count(x => x.Goals1 < x.Goals2),
                    GA = x.Sum(x => x.Goals2),
                    GF = x.Sum(x => x.Goals1)
                })
                .Union(_gameContext.Items
                    .Where(x => x.Status == GameStatus.Completed)
                    .GroupBy(x => new { x.Team2 })
                    .Select(x => new StatsDTO
                    {
                        Id = x.Key.Team2,                        
                        Games = x.Count(),
                        Wins = x.Count(x => x.Goals2 > x.Goals1),
                        Losses = x.Count(x => x.Goals2 < x.Goals1),
                        GA = x.Sum(x => x.Goals1),
                        GF = x.Sum(x => x.Goals2)
                    }))
                .GroupBy(x => new { x.Id })
                .Select(x => new StatsDTO
                {
                    Id = x.Key.Id,                    
                    Games = x.Sum(x => x.Games),
                    Wins = x.Sum(x => x.Wins),
                    Losses = x.Sum(x => x.Losses),
                    GA = x.Sum(x => x.GA),
                    GF = x.Sum(x => x.GF)
                })
                .OrderByDescending(x => (double)x.Wins / x.Games)
                .ThenByDescending(x => x.GF - x.GA)
                .ToList();

            var teamIds = stats.Select(x => x.Id).ToList();
            var teams = _teamContext.Items.Where(x => teamIds.Contains(x.Id)).ToList().ToDictionary(x => x.Id);
            foreach (var s in stats) {
                s.Name = teams.ContainsKey(s.Id) ? teams[s.Id].Name : s.Id.ToString();
            }
        
            return Ok(stats);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    
}