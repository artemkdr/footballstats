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

abstract public class GameDTOBase {
    public int Id { get; set; }    

    public string? Status { get; set; }
        
    public int Goals1 { get; set; }    
    
    public int Goals2 { get; set; }    
    
    public DateTime? CompleteDate { get; set; }
    
    public JsonDocument? Vars { get; set; }
}

public class GameDTOLight {
    public int Id { get; set; }
}

public class GameDTO : GameDTOBase
{    
    public int Team1 { get; set; }    
    
    public int Team2 { get; set; }    
}

public class GameDTOExtended : GameDTOBase
{   
    public TeamDTOLight? Team1 { get; set; }    
    
    public TeamDTOLight? Team2 { get; set; }    
}

public class GameDTOFull : GameDTOBase
{   
    public TeamDTO? Team1 { get; set; }    
    
    public TeamDTO? Team2 { get; set; }    
}


public class GameDTOSpecial : GameDTOBase
{   
    public TeamDTOExtended? Team1 { get; set; }    
    
    public TeamDTOExtended? Team2 { get; set; }    
}

[ApiController]
public class GameController : BaseController
{
    public GameController(IConfiguration configuration, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext, BaseDBContext<Game> gameContext) 
        : base(configuration, userContext, teamContext, gameContext) 
    {
    }

    [Route("game/{id}")]    
    [HttpGet]
    public IActionResult GetGame(int id)
    {
        try {
            var item = _gameContext.Items.Find(id);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Game '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
            
            var team1 = _teamContext.Items.Find(item.Team1);
            var team2 = _teamContext.Items.Find(item.Team2);

            var dto = new GameDTOFull() {
                Id = item.Id,
                Team1 = new TeamDTO() {
                    Id = item.Team1,
                    Name = team1?.Name,
                    Players = team1?.Players,
                    Status = team1?.Status.ToString()
                },
                Team2 = new TeamDTO() {
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
            };
            return Ok(dto);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }          
    }

    [Route("game")]
    [HttpGet]
    public IActionResult GetGames(int? team1 = null, int? team2 = null, DateTime? fromDate = null, DateTime? toDate = null, string? status = null, string? players = null) 
    {
        try {
            var query = _gameContext.Items.AsQueryable();
            if (!string.IsNullOrEmpty(status)) {
                if (Enum.TryParse<GameStatus>(status, true, out var statusEnum))
                    query = query.Where(x => x.Status == statusEnum);
                else 
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"status '{status}' doesn't exist"));
            }

            if (players != null) {
                string[] playersArray = players.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var playersTeams = _teamContext.Items.Where(x => x.Players != null && playersArray.All(p => x.Players.Contains(p))).Select(x => x.Id).ToList();
                if (playersTeams != null && playersTeams.Count > 0) {
                    query = query.Where(x => playersTeams.Contains(x.Team1) || playersTeams.Contains(x.Team2));
                }
                //query = query.Where(x => x.Players != null && playersArray.All(p => x.Players.Contains(p)));
            } else {
                if (team1.HasValue && team2.HasValue) {
                    query = query.Where(x => x.Team1 == team1.Value && x.Team2 == team2.Value || x.Team1 == team2.Value && x.Team2 == team1.Value);
                } else if (team1.HasValue) {
                    query = query.Where(x => x.Team1 == team1.Value || x.Team2 == team1.Value);
                } else if (team2.HasValue) {
                    query = query.Where(x => x.Team1 == team2.Value || x.Team2 == team2.Value);
                }
            }

            if (fromDate.HasValue)
                query = query.Where(x => x.CompleteDate != null && x.CompleteDate.Value.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.CompleteDate != null && x.CompleteDate.Value.Date <= toDate.Value.Date);

            var items = query                    
                    .OrderBy(x => x.Status == GameStatus.Playing ? 0 : 
                                 x.Status == GameStatus.Completed ? 1 :
                                 x.Status == GameStatus.NotStarted ? 2 : 3) // Order by Status priority
                    .ThenByDescending(x => x.Status == GameStatus.Playing ? x.ModifyDate :
                                         x.Status == GameStatus.Completed ? x.CompleteDate : x.ModifyDate)
                    .Take(LIST_LIMIT)                    
                    .ToList();
            
            var teamIds = items.SelectMany(x => new[] { x.Team1, x.Team2 }).Distinct().ToList();
            var teams = _teamContext.Items.Where(x => teamIds.Contains(x.Id)).ToList().ToDictionary(x => x.Id);
            var itemsWithTeams = new List<GameDTOSpecial>();
            foreach (var t in items) {
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
                
                itemsWithTeams.Add(new GameDTOSpecial() {
                    Id = t.Id,
                    Team1 = new TeamDTOExtended() {
                        Id = t1.Id,
                        Name = t1.Name,
                        Players = t1Players.ToArray(),
                        Status = t1.Status.ToString()
                    },
                    Team2 = new TeamDTOExtended() {
                        Id = t2.Id,
                        Name = t2.Name,
                        Players = t2Players.ToArray(),
                        Status = t2.Status.ToString()
                    },
                    Status = t.Status.ToString(),
                    CompleteDate = t.CompleteDate,
                    Goals1 = t.Goals1,
                    Goals2 = t.Goals2                        
                });
            }
            return Ok(itemsWithTeams);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("game")]
    [HttpPost]
    public IActionResult CreateGame(GameDTO data)
    {
        if (data == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game is missing"));        
        if (data.Team1 <= 0)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game team1 is missing"));
        if (data.Team2 <= 0)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game team2 is missing"));
        if (data.Team1 == data.Team2)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game team1 and team2 must be different"));

        // TODO: add data validation
        // TODO: check goals are positive values
        // TODO: check that the teams exist        
                
        var newItem = new Game {
            Team1 = data.Team1,
            Team2 = data.Team2,
            Goals1 = data.Goals1,
            Goals2 = data.Goals2,            
            Vars = data.Vars,            
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = GameStatus.NotStarted
        };
        if (data.Status != null && Enum.TryParse<GameStatus>(data.Status, out var status))
            newItem.Status = status;
        if (data.CompleteDate != null)
            newItem.CompleteDate = (DateTime)data.CompleteDate;

        try {
            _gameContext.Items.Add(newItem);
            _gameContext.SaveChanges();
            return CreatedAtAction(nameof(GetGame), new { id = newItem.Id }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("game/{id}")]
    [HttpPost]
    public IActionResult UpdateGame(int id, GameDTO data)
    {
        var item = _gameContext.Items.Find(id);
        if (item == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Game '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
        
        // TODO: add data validation
        // TODO: check goals are positive values
        // TODO: check that the teams exist       

        if (data != null) {
            if (data.Goals1 > 0)
                item.Goals1 = data.Goals1;
            if (data.Goals2 > 0)
                item.Goals2 = data.Goals2;   
            if (data.Team1 > 0)
                item.Team1 = data.Team1;
            if (data.Team2 > 0)
                item.Team2 = data.Team2;                     
            if (data.Status != null && Enum.TryParse<GameStatus>(data.Status, out var status)) {
                // if the status is setting to Completed and there is no explicit CompleteDate received, 
                // then set it automatically
                if (status == GameStatus.Completed && item.Status < GameStatus.Completed && !data.CompleteDate.HasValue && !item.CompleteDate.HasValue) {
                    item.CompleteDate = DateTime.UtcNow;
                }
                item.Status = status;
            }
            if (data.CompleteDate.HasValue) {                
                item.CompleteDate = data.CompleteDate;                
            }
            
            item.ModifyDate = DateTime.UtcNow; 

            try {
                _gameContext.Items.Update(item);
                _gameContext.SaveChanges();            
                var dto = new GameDTOLight() {
                    Id = item.Id
                };
                return Ok(dto);
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
            }
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no game data provided"));
    }
}