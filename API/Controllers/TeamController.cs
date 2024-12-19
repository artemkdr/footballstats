using System.Net;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TeamDTO : IBaseDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }    
    
    public string? Status { get; set; }

    public string[]? Players { get; set; }    

    public UserDTO[]? PlayerDetails { get; set; }    
}


[ApiController]
public class TeamController : BaseController
{
    private readonly IUserService _userService; // Inject the service

    private readonly ITeamService _teamService; // Inject the service

    public TeamController(
        IConfiguration configuration, 
        BaseDBContext<User> userContext, 
        BaseDBContext<Team> teamContext, 
        BaseDBContext<Game> gameContext,
        IUserService userService,
        ITeamService teamService) 
        : base(configuration, userContext, teamContext, gameContext)
    {
        _teamService = teamService;
        _userService = userService;
    }    
    

    [Route("teams/{id}")]    
    [HttpGet]
    public async Task<IActionResult> GetTeam(int id)
    {   
        var item = await _teamService.GetTeamAsync(id);
        if (item == null)
            return NotFoundProblem($"Team '{id}' not found");

        // get players details
        var players = new List<UserDTO>();
        if (item.Players != null) {
            var users = await _userService.GetUsersAsync(item.Players);
            players = users.Select(x => new UserDTO() {
                Username = x.Username,
                Vars = x.Vars,
                Status = x.Status.ToString()
            }).ToList();
            if (players.Count != item.Players.Length) { // complete players list with 'broken' user references
                foreach (var un in item.Players) {
                    if (players.Find(x => x.Username == un) == null) {
                        players.Add(new UserDTO() {
                            Username = un
                        });
                    }
                }
            }
        }
        var dto = new TeamDTO() {
            Id = item.Id,
            Name = item.Name,
            Players = players.Select(x => x.Username!).ToArray(),
            PlayerDetails = players.ToArray(),
            Status = item.Status.ToString()
        };
        return Ok(dto);        
    }

    [Route("teams")]
    [HttpGet]
    public async Task<IActionResult> GetTeams(string? name = null, string? status = null, string? players = null, int page = 1, int limit = 0) 
    {
        if (!string.IsNullOrEmpty(status) && !Enum.TryParse<TeamStatus>(status, true, out var statusEnum))
            return BadRequestProblem($"status '{status}' doesn't exist");
        
        var teamsPages = await _teamService.GetTeamsAsync(name, status, players, page, limit);

        return Ok(new ListDTO() {
            List = teamsPages.List?.Select(x => new TeamDTO() {
                Id = x.Id,
                Name = x.Name,                
                Status = x.Status.ToString()
            }).ToArray(),
            Page = teamsPages.Page,
            PageSize = teamsPages.PageSize,
            Total = teamsPages.Total,
            TotalPages = teamsPages.TotalPages
        });
    }

    [Route("teams")]
    [HttpPost]
    public async Task<IActionResult> CreateTeam(TeamDTO data)
    {
        try {
            var newItem = await _teamService.CreateTeamAsync(data);
            return CreatedAtAction(nameof(GetTeam), new { id = newItem.Id }, newItem);
        } catch (InvalidTeamException ex) {
            return BadRequestProblem(ex.Message);
        }       
    }

    [Route("teams/{id}")]
    [HttpPatch]
    public async Task<IActionResult> UpdateTeam(int id, TeamDTO data)
    {
        try {
            var item = await _teamService.UpdateTeamAsync(id, data);
            if (item == null)
                return NotFoundProblem($"Team '{id}' not found");
            var dto = new TeamDTO() {
                Id = item.Id,
                Name = item.Name,
                Status = item.Status.ToString()
            };
            return Ok(dto);     

        } catch (InvalidTeamException ex) {
            return BadRequestProblem(ex.Message);
        } catch (InvalidOperationException) {
            return Problem($"team name {data.Name} is already used");
        }    
    }
}