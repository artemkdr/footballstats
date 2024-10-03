using System.Net;
using API.Data;
using API.Models;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers;

public class TeamModel
{
    public string? Name { get; set; }
    
    public string[]? Players { get; set; }    

    public string? Status { get; set; }
}

[ApiController]
public class TeamController : BaseController
{
    public TeamController(IConfiguration configuration, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext, BaseDBContext<Game> gameContext) 
        : base(configuration, userContext, teamContext, gameContext)
    {
    }    
    

    [Route("team/{id}")]    
    [HttpGet]
    public IActionResult GetTeam(int id)
    {
        try {
            var item = _teamContext.Items.Find(id);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Team '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
            return RequestHelpers.Success(RequestHelpers.ToDict("id", id, "name", item.Name ?? ""));
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }        
    }

    [Route("team")]
    [HttpGet]
    public IActionResult GetTeams(string? name = null, string? status = null, string? players = null) 
    {
        try {
            var query = _teamContext.Items.AsQueryable();

            // case insensitive "like '%value%'" search by name
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name != null && x.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(status))
                if (Enum.TryParse<TeamStatus>(status, true, out var statusEnum))
                    query = query.Where(x => x.Status == statusEnum);
                else 
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"status '{status}' doesn't exist"));                

            if (players != null) {
                string[] playersArray = players.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                query = query.Where(x => x.Players != null && playersArray.All(p => x.Players.Contains(p)));
            }
            
            var items = query.OrderBy(x => x.Name)
                            .Take(LIST_LIMIT)
                            .ToList();

            return Ok(items);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("team")]
    [HttpPost]
    public IActionResult CreateTeam(TeamModel data)
    {
        try {
            ValidateTeamData(data, true);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }
        
        var newItem = new Team {
            Name = data.Name,
            Players = data.Players,
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = TeamStatus.Active
        };
        if (data.Status != null && Enum.TryParse<TeamStatus>(data.Status, out var status))
            newItem.Status = status;

        try {
            _teamContext.Items.Add(newItem);
            _teamContext.SaveChanges();
            return CreatedAtAction(nameof(GetTeam), new { id = newItem.Id }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("team/{id}")]
    [HttpPost]
    public IActionResult UpdateTeam(int id, TeamModel data)
    {
        var item = _teamContext.Items.Find(id);
        if (item == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Team '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
        
        try {
            ValidateTeamData(data);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }
        
        if (data != null) {
            if (data.Name != null && data.Name != item.Name) {                
                if (_teamContext.Items.FirstOrDefault(x => x.Name == data.Name) != null)
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"team name {data.Name} is already used"));
                item.Name = data.Name;
            }

            if (data.Players != null)
                item.Players = data.Players;
            
            if (data.Status != null && Enum.TryParse<TeamStatus>(data.Status, out var status))
                item.Status = status;
                        
            item.ModifyDate = DateTime.UtcNow; 
            try {
                _teamContext.Items.Update(item);
                _teamContext.SaveChanges();            
                return RequestHelpers.Success(RequestHelpers.ToDict("id", item.Id));
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
            }
        }

        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no team data provided"));
    }

    private void ValidateTeamData(TeamModel data, bool create = false) {
        if (data == null)
            throw new Exception("team data is empty");
                
        if (data != null) {
            if (create && data.Name == null) 
                throw new Exception("team name is missing");        

            if (data.Players == null) 
                throw new Exception("team players are missing");            
            
            if (data.Players.Length < MIN_TEAM_PLAYERS) 
                throw new Exception($"team must have min {MIN_TEAM_PLAYERS} players");

            if (data.Players.Length > MAX_TEAM_PLAYERS) 
                throw new Exception($"team must have max {MAX_TEAM_PLAYERS} players");

            foreach (var p in data.Players) {
                if (_userContext.Items.Find(p) == null) 
                    throw new Exception($"team player {p} doesn't exist");
            }
        }
    }
}