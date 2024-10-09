using API.Data;
using API.Models;
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
    public StatsController(IConfiguration configuration, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext, BaseDBContext<Game> gameContext) 
        : base(configuration, userContext, teamContext, gameContext)
    {
    }

    [Route("stats")]    
    [HttpGet]
    public IActionResult GetStats(int? team = null, string? sort = "default")
    {
        try {
            var query = _gameContext.Items.AsQueryable();
            if (team != null && team.Value > 0) {
                query = query.Where(x => x.Team1 == team.Value || x.Team2 == team.Value);
            }
            var grouppedStats = query
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
                });
            if (team != null && team.Value > 0) {
                grouppedStats = grouppedStats.Where(x => x.Id == team.Value);
            }
            var stats = grouppedStats
                .OrderByDescending(x => (double)x.Wins / x.Games)
                .ThenByDescending(x => x.GF - x.GA)
                .ToList();

            // fill the team names from the teams table
            var teamIds = stats.Select(x => x.Id).ToList();
            var teams = _teamContext.Items.Where(x => teamIds.Contains(x.Id)).ToList().ToDictionary(x => x.Id);
            foreach (var s in stats) {
                s.Name = teams.ContainsKey(s.Id) ? teams[s.Id].Name : s.Id.ToString();
            }
        
            return Ok(stats);
        } catch (Exception ex) {
            return Problem(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [Route("statsrivals")]    
    [HttpGet]
    public IActionResult GetStatsRivals(int team1, int team2)
    {
        try {
            var query = _gameContext.Items.AsQueryable();            
            query = query.Where(x => x.Status == GameStatus.Completed && (x.Team1 == team1 && x.Team2 == team2 || x.Team1 == team2 && x.Team2 == team1));
            var games = query.ToList();
            var stats = new StatsRivalsDTO { Team1 = team1, Team2 = team2 };
            foreach (var g in games) {
                if (g.Goals1 == g.Goals2) stats.Draws++;
                else {
                    var winner = g.Goals1 > g.Goals2 ? g.Team1 : g.Team2;
                    if (winner == team1)
                        stats.Wins1++;
                    else
                        stats.Wins2++;
                }
            }
            return Ok(stats);
        } catch (Exception ex) {
            return Problem(ex.InnerException?.Message ?? ex.Message);
        }
    }

    
}