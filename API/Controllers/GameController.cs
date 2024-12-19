using System.Net;
using System.Text.Json;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class GameDTO : IBaseDTO
{    
    public int Id { get; set; }    

    public string? Status { get; set; }
        
    public int Goals1 { get; set; }    
    
    public int Goals2 { get; set; }    

    public int Team1 { get; set; }    
    
    public int Team2 { get; set; }    

    public TeamDTO? Team1Detail { get; set; }    
    
    public TeamDTO? Team2Detail { get; set; }    
    
    public DateTime? CompleteDate { get; set; }
    
    public JsonDocument? Vars { get; set; }

    
}

[ApiController]
public class GameController : BaseController
{
    private readonly IUserService _userService; // Inject the service

    private readonly ITeamService _teamService; // Inject the service

    private readonly IGameService _gameService; // Inject the service

    public GameController(
        IConfiguration configuration, 
        BaseDBContext<User> userContext, 
        BaseDBContext<Team> teamContext, 
        BaseDBContext<Game> gameContext,
        IGameService gameService,
        ITeamService teamService,
        IUserService userService) 
        : base(configuration, userContext, teamContext, gameContext) 
    {
        _userService = userService;
        _teamService = teamService;
        _gameService = gameService;
    }

    [Route("games/{id}")]    
    [HttpGet]
    public async Task<IActionResult> GetGame(int id)
    {        
        var item = await _gameService.GetGameAsync(id);
        if (item == null)
            return NotFoundProblem($"Game '{id}' not found");
        
        var team1 = await _teamService.GetTeamAsync(item.Team1);
        var team2 = await _teamService.GetTeamAsync(item.Team2);

        return Ok(new GameDTO() {
            Id = item.Id,
            Team1 = item.Team1,
            Team1Detail = new TeamDTO() {
                Id = item.Team1,
                Name = team1?.Name,
                Players = team1?.Players,
                Status = team1?.Status.ToString()
            },
            Team2 = item.Team2,
            Team2Detail = new TeamDTO() {
                Id = item.Team2,
                Name = team2?.Name,
                Players = team2?.Players,
                Status = team2?.Status.ToString()
            },
            Status = item.Status.ToString(),
            CompleteDate = item.CompleteDate,
            Goals1 = item.Goals1,
            Goals2 = item.Goals2,
            Vars = item.Vars
        });        
    }

    [Route("games")]
    [HttpGet]
    public async Task<IActionResult> GetGames(
        int? team1 = null, int? team2 = null, DateTime? fromDate = null, DateTime? toDate = null, string? status = null, string? players = null,
        int page = 1, int limit = 0) 
    {
        if (!string.IsNullOrEmpty(status) && !Enum.TryParse<GameStatus>(status, true, out var statusEnum))
            return BadRequestProblem($"status '{status}' doesn't exist");

        var gamesPages = await _gameService.GetGamesAsync(team1, team2, fromDate, toDate, status, players, page, limit);
        var games = gamesPages.List;
        var gamesWithTeams = new List<GameDTO>();

        // fill games with the teams details
        if (games != null) {
            var teamIds = games.SelectMany(x => new[] { x.Team1, x.Team2 }).Distinct().ToList();
            var teams = (await _teamContext.Items.Where(x => teamIds.Contains(x.Id)).ToListAsync()).ToDictionary(x => x.Id);                        
            foreach (var t in games) {
                var t1 = teams.ContainsKey(t.Team1) ? teams[t.Team1] : new Team() { Id = t.Team1 };
                var t2 = teams.ContainsKey(t.Team2) ? teams[t.Team2] : new Team() { Id = t.Team2 };
                var t1Players = new List<UserDTO>();
                var t2Players = new List<UserDTO>();
                if (t1.Players != null) 
                    foreach (var u in t1.Players) 
                        t1Players.Add(new UserDTO() { Username = u });
                if (t2.Players != null) 
                    foreach (var u in t2.Players) 
                        t2Players.Add(new UserDTO() { Username = u });                    
                
                gamesWithTeams.Add(new GameDTO() {
                    Id = t.Id,
                    Team1 = t1.Id,
                    Team1Detail = new TeamDTO() {
                        Id = t1.Id,
                        Name = t1.Name,
                        Players = t1Players.Select(x => x.Username!).ToArray(),
                        PlayerDetails = t1Players.ToArray(),
                        Status = t1.Status.ToString()
                    },
                    Team2 = t2.Id,
                    Team2Detail = new TeamDTO() {
                        Id = t2.Id,
                        Name = t2.Name,
                        Players = t2Players.Select(x => x.Username!).ToArray(),
                        PlayerDetails = t2Players.ToArray(),
                        Status = t2.Status.ToString()
                    },
                    Status = t.Status.ToString(),
                    CompleteDate = t.CompleteDate,
                    Goals1 = t.Goals1,
                    Goals2 = t.Goals2                        
                });
            }
        }

        return Ok(new ListDTO {
            Page = gamesPages.Page,
            PageSize = gamesPages.PageSize,
            Total = gamesPages.Total,
            TotalPages = gamesPages.TotalPages,
            List = gamesWithTeams?.ToArray()
        });        
    }

    [Route("games")]
    [HttpPost]
    public async Task<IActionResult> CreateGame(GameDTO data)
    {
        try {
            var newItem = await _gameService.CreateGameAsync(data);
            return CreatedAtAction(nameof(GetGame), new { id = newItem.Id }, newItem);
        } catch (InvalidGameException ex) {
            return BadRequestProblem(ex.Message);
        }
    }

    [Route("games/{id}")]
    [HttpPatch]
    public async Task<IActionResult> UpdateGame(int id, GameDTO data)
    {
        try {
            var item = await _gameService.UpdateGameAsync(id, data);
            if (item == null)
                return NotFoundProblem($"Game '{id}' not found");
            var dto = new GameDTO() {
                Id = item.Id,                
                Status = item.Status.ToString()
            };
            return Ok(dto);     

        } catch (InvalidGameException ex) {
            return BadRequestProblem(ex.Message);
        }            
    }
}