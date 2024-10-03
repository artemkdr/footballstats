using System.Net;
using System.Text.Json;
using API.Data;
using API.Models;
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
    
    public DateTime? CompleteDate { get; set; }
    
    public JsonDocument? Vars { get; set; }
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
            return RequestHelpers.Success(RequestHelpers.ToDict("id", id));
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }          
    }

    [Route("game")]
    [HttpGet]
    public IActionResult GetGames(int? team1 = null, int? team2 = null, DateTime? fromDate = null, DateTime? toDate = null, string? status = null) 
    {
        try {
            var query = _gameContext.Items.AsQueryable();
            if (!string.IsNullOrEmpty(status)) {
                if (Enum.TryParse<GameStatus>(status, true, out var statusEnum))
                    query = query.Where(x => x.Status == statusEnum);
                else 
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"status '{status}' doesn't exist"));
            }

            if (team1.HasValue)
                query = query.Where(x => x.Team1 == team1.Value);

            if (team2.HasValue)
                query = query.Where(x => x.Team2 == team2.Value);

            if (fromDate.HasValue)
                query = query.Where(x => x.CompleteDate != null && x.CompleteDate.Value.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.CompleteDate != null && x.CompleteDate.Value.Date <= toDate.Value.Date);

            var items = query.
                    OrderBy(x => x.Status == GameStatus.Playing ? 0 : 
                                 x.Status == GameStatus.Completed ? 1 :
                                 x.Status == GameStatus.NotStarted ? 2 : 3) // Order by Status priority
                    .ThenByDescending(x => x.Status == GameStatus.Playing ? x.ModifyDate :
                                         x.Status == GameStatus.Completed ? x.CompleteDate : x.ModifyDate)
                    .Take(LIST_LIMIT)
                    .ToList();

            return Ok(items);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("game")]
    [HttpPost]
    public IActionResult CreateGame(GameModel data)
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
    public IActionResult UpdateGame(int id, GameModel data)
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
                return RequestHelpers.Success(RequestHelpers.ToDict("id", item.Id));
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
            }
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no game data provided"));
    }
}