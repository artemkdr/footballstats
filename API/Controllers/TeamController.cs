using System.Net;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TeamDTOLight : IBaseDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }    
    
    public string? Status { get; set; }
}

public class TeamDTO : TeamDTOLight
{    
    public string[]? Players { get; set; }    
}

public class TeamDTOExtended : TeamDTOLight
{    
    public UserDTO[]? Players { get; set; }    
}

public class TeamDTOSpecial : TeamDTOLight
{    
    public UserDTOLight[]? Players { get; set; }    
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
                return NotFound(new ErrorDTO($"Team '{id}' not found"));

            var players = new List<UserDTO>();
            if (item.Players != null) {
                players = _userContext.Items.Where(x => item.Players.Contains(x.Username)).Select(x => new UserDTO() {
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
            var dto = new TeamDTOExtended() {
                Id = item.Id,
                Name = item.Name,
                Players = players.ToArray(),
                Status = item.Status.ToString()
            };
            return Ok(dto);
        } catch (Exception ex) {
            return Problem(ex.InnerException?.Message ?? ex.Message);
        }        
    }

    [Route("team")]
    [HttpGet]
    public IActionResult GetTeams(string? name = null, string? status = null, string? players = null, int page = 1, int limit = 0) 
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = Math.Max(limit, LIST_LIMIT);

        try {
            var query = _teamContext.Items.AsQueryable();

            // case insensitive "like '%value%'" search by name
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name != null && x.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(status))
                if (Enum.TryParse<TeamStatus>(status, true, out var statusEnum))
                    query = query.Where(x => x.Status == statusEnum);
                else 
                    return BadRequest(new ErrorDTO($"status '{status}' doesn't exist"));

            if (players != null) {
                string[] playersArray = players.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                query = query.Where(x => x.Players != null && playersArray.All(p => x.Players.Contains(p)));
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / limit);
            if (page > totalPages) page = Math.Max(1, totalPages);
            
            var items = query.OrderBy(x => x.Name)
                            .Skip((page - 1) * limit)
                            .Take(limit)
                            .Select(x => new TeamDTOLight() {
                                Id = x.Id,
                                Name = x.Name,
                                Status = x.Status.ToString()
                            })
                            .ToList();

            return Ok(new ListDTO {
                Page = page,
                PageSize = limit,
                Total = totalCount,
                TotalPages = totalPages,
                List = items.ToArray()
            });
        } catch (Exception ex) {
            return Problem(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [Route("team")]
    [HttpPost]
    public IActionResult CreateTeam(TeamDTO data)
    {
        try {
            ValidateTeamData(data, true);
        } catch (Exception ex) {
            return BadRequest(new ErrorDTO(ex.Message));
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
            return Problem(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [Route("team/{id}")]
    [HttpPost]
    public IActionResult UpdateTeam(int id, TeamDTO data)
    {
        var item = _teamContext.Items.Find(id);
        if (item == null)
            return NotFound(new ErrorDTO($"Team '{id}' not found"));
        
        try {
            ValidateTeamData(data);
        } catch (Exception ex) {
            return BadRequest(new ErrorDTO(ex.Message));
        }
        
        if (data != null) {
            if (data.Name != null && data.Name != item.Name) {                
                if (_teamContext.Items.FirstOrDefault(x => x.Name == data.Name) != null)
                    return Problem($"team name {data.Name} is already used");
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
                var dto = new TeamDTOLight() {
                    Id = item.Id,
                    Name = item.Name,
                    Status = item.Status.ToString()
                };
                return Ok(dto);
            } catch (Exception ex) {
                return Problem(ex.InnerException?.Message ?? ex.Message);
            }
        }

        return BadRequest(new ErrorDTO("no team data provided"));
    }

    private void ValidateTeamData(TeamDTO data, bool create = false) {
        if (data == null)
            throw new Exception("team data is empty");
                
        if (data != null) {
            if (create && data.Name == null) 
                throw new Exception("team name is missing");        
            
            if (create && data.Players == null) 
                throw new Exception("team players are missing");            
            
            if (data.Players != null) {
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
}