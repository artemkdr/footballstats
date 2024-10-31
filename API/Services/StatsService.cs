using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public interface IStatsService 
{
    public Task<List<StatsDTO>> GetStatsAsync (int? team, string? sort);
    public Task<StatsRivalsDTO> GetStatsRivalsAsync (int team1, int team2);
}

public class StatsService : IStatsService {
    
    private readonly BaseDBContext<Game> _gameContext; 

    private readonly BaseDBContext<Team> _teamContext; 

    public StatsService(BaseDBContext<Game> gameContext, BaseDBContext<Team> teamContext)
    {
        _gameContext = gameContext;
        _teamContext = teamContext;
    }

    public async Task<List<StatsDTO>> GetStatsAsync(int? team = null, string? sort = "default") {
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
        var stats = await grouppedStats
            .OrderByDescending(x => (double)x.Wins / x.Games)
            .ThenByDescending(x => x.GF - x.GA)
            .ToListAsync();

        // fill the team names from the teams table
        var teamIds = stats.Select(x => x.Id).ToList();
        var teams = (await _teamContext.Items.Where(x => teamIds.Contains(x.Id)).ToListAsync()).ToDictionary(x => x.Id);
        foreach (var s in stats) {
            s.Name = teams.ContainsKey(s.Id) ? teams[s.Id].Name : s.Id.ToString();
        }

        return stats;
    }

    
    public async Task<StatsRivalsDTO> GetStatsRivalsAsync(int team1, int team2) {
        var query = _gameContext.Items.AsQueryable();            
        query = query.Where(x => x.Status == GameStatus.Completed && (x.Team1 == team1 && x.Team2 == team2 || x.Team1 == team2 && x.Team2 == team1));
        var games = await query.ToListAsync();
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
        return stats;
    }
}