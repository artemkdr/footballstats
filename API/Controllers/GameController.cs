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
    public DateTime? CompletedDate { get; set; }
    public JsonDocument? Vars { get; set; }
}

[ApiController]
public class GameController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;

    private readonly GameContext _context;
    
    public GameController(IConfiguration configuration, GameContext context)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
        _context = context;
    }

    [Route("game/{id}")]    
    [HttpGet]
    public IActionResult GetGame(int id)
    {
        try {
            var item = _context.Games.Find(id);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Game '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
            return RequestHelpers.Success(RequestHelpers.ToDict("id", id));
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }          
    }

    [Route("game")]
    [HttpPost]
    public IActionResult CreateGame(GameModel data)
    {
        if (data == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game is missing"));
        if (data.Status == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game status is missing"));
        if (data.Team1 <= 0)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game team1 is missing"));
        if (data.Team2 <= 0)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "game team2 is missing"));

        // TODO: add data validation
        // TODO: check goals are positive values
        // TODO: check that the teams exist        
                
        var newItem = new Game {
            Team1 = data.Team1,
            Team2 = data.Team2,
            Goals1 = data.Goals1,
            Goals2 = data.Goals2,
            Status = data.Status,
            Vars = data.Vars,            
            Createdate = DateTime.UtcNow,            
            Modifydate = DateTime.UtcNow
        };
        if (data.CompletedDate != null)
            newItem.CompletedDate = (DateTime)data.CompletedDate;

        try {
            _context.Games.Add(newItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetGame), new { id = newItem.Id }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }
    }

    [Route("game/{id}")]
    [HttpPost]
    public IActionResult UpdateGame(int id, GameModel data)
    {
        var item = _context.Games.Find(id);
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
            if (data.Status != null)
                item.Status = data.Status;
            if (data.CompletedDate != null)
                item.CompletedDate = (DateTime)data.CompletedDate;
            
            item.Modifydate = DateTime.UtcNow; 

            try {
                _context.Games.Update(item);
                _context.SaveChanges();            
                return RequestHelpers.Success(RequestHelpers.ToDict("id", item.Id));
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
            }
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no game data provided"));
    }
}