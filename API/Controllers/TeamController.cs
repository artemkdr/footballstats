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
}

[ApiController]
public class TeamController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;

    private readonly TeamContext _context;
    
    public TeamController(IConfiguration configuration, TeamContext context)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
        _context = context;
    }

    [Route("team/{id}")]    
    [HttpGet]
    public IActionResult GetTeam(int id)
    {
        try {
            var item = _context.Teams.Find(id);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Team '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
            return RequestHelpers.Success(RequestHelpers.ToDict("id", id, "name", item.Name ?? ""));
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }        
    }

    [Route("team")]
    [HttpPost]
    public IActionResult CreateTeam(TeamModel data)
    {
        if (data == null || data.Name == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "team name is missing"));

        // TODO: add data validation            
        // TODO: check that the name is a valid name
        // TODO: check that the players exist 
        
        var newItem = new Team {
            Name = data.Name,
            Players = data.Players,
            Createdate = DateTime.UtcNow,            
            Modifydate = DateTime.UtcNow
        };

        try {
            _context.Teams.Add(newItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTeam), new { id = newItem.Id }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }
    }

    [Route("team/{id}")]
    [HttpPost]
    public IActionResult UpdateTeam(int id, TeamModel data)
    {
        var item = _context.Teams.Find(id);
        if (item == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"Team '{id}' not found"), Response, (int)HttpStatusCode.NotFound);
        
        if (data != null) {
            
            // TODO: add data validation            
            // TODO: check that the name is a valid name
            // TODO: check that the players exist   

            if (data.Name != null && data.Name != item.Name) {                
                if (_context.Teams.FirstOrDefault(x => x.Name == data.Name) != null)
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"team name {data.Name} is already used"));
                item.Name = data.Name;
            }

            if (data.Players != null)
                item.Players = data.Players;
                        
            item.Modifydate = DateTime.UtcNow; 
            try {
                _context.Teams.Update(item);
                _context.SaveChanges();            
                return RequestHelpers.Success(RequestHelpers.ToDict("id", item.Id));
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
            }
        }

        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no team data provided"));
    }
}