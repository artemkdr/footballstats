using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public interface ITeamService 
{
    Task<Team?> GetTeamAsync(int id);
    Task<PagedList<Team>> GetTeamsAsync(string? name, string? status, string? players, int page, int limit);
    Task<Team> CreateTeamAsync(TeamDTO data);
    Task<Team?> UpdateTeamAsync(int id, TeamDTO data);
}

public class TeamService : ITeamService {

    public static readonly int LIST_LIMIT = 500;

    public static int MAX_TEAM_PLAYERS = 2;
    
    public static int MIN_TEAM_PLAYERS = 1;
    
    
    private readonly BaseDBContext<User> _userContext;
    private readonly BaseDBContext<Team> _teamContext; 

    public TeamService(BaseDBContext<User> userContext, BaseDBContext<Team> teamContext)
    {
        _userContext = userContext;
        _teamContext = teamContext;
    }

    public async Task<Team> CreateTeamAsync(TeamDTO data)
    {        
        ValidateTeamData(data, true);
        
        var newItem = new Team {
            Name = data.Name,
            Players = data.Players,
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = TeamStatus.Active
        };
        if (data.Status != null && Enum.TryParse<TeamStatus>(data.Status, out var status))
            newItem.Status = status;
        
        _teamContext.Items.Add(newItem);
        await _teamContext.SaveChangesAsync();        
        return newItem;
    }

    public async Task<Team?> GetTeamAsync(int id)
    {
        var item = await _teamContext.Items.FindAsync(id);        
        return item;
    }

    public async Task<PagedList<Team>> GetTeamsAsync(string? name, string? status, string? players, int page, int limit)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = Math.Max(limit, UserService.LIST_LIMIT);
        
        var query = _teamContext.Items.AsQueryable();

        // case insensitive "like '%value%'" search by name
        if (!string.IsNullOrEmpty(name))
            query = query.Where(x => x.Name != null && x.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TeamStatus>(status, true, out var statusEnum))
            query = query.Where(x => x.Status == statusEnum);
            
        if (players != null) {
            string[] playersArray = players.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            query = query.Where(x => x.Players != null && playersArray.All(p => x.Players.Contains(p)));
        }

        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / limit);
        if (page > totalPages) page = Math.Max(1, totalPages);
        
        var items = await query.OrderBy(x => x.Name)
                        .Skip((page - 1) * limit)
                        .Take(limit)                        
                        .ToListAsync();

        return new PagedList<Team> {
            Page = page,
            PageSize = limit,
            Total = totalCount,
            TotalPages = totalPages,
            List = items
        };
    }

    public async Task<Team?> UpdateTeamAsync(int id, TeamDTO data)
    {
        var item = await _teamContext.Items.FindAsync(id);
        if (item == null)
            return null;
                
        ValidateTeamData(data);
        
        if (data.Name != null && data.Name != item.Name) {                
            if (await _teamContext.Items.FirstOrDefaultAsync(x => x.Name == data.Name) != null)
                throw new InvalidOperationException($"team name {data.Name} is already used");

            item.Name = data.Name;
        }

        if (data.Players != null)
            item.Players = data.Players;
        
        if (data.Status != null && Enum.TryParse<TeamStatus>(data.Status, out var status))
            item.Status = status;
                    
        item.ModifyDate = DateTime.UtcNow; 
        
        _teamContext.Items.Update(item);
        _teamContext.SaveChanges();            
        return item;                
    }

    private void ValidateTeamData(TeamDTO data, bool create = false) {
        if (data == null)
            throw new InvalidTeamException("team data is empty");
                
        if (data != null) {
            if (create && data.Name == null) 
                throw new InvalidTeamException("team name is missing");        
            
            if (create && data.Players == null) 
                throw new InvalidTeamException("team players are missing");            
            
            if (data.Players != null) {
                if (data.Players.Length < MIN_TEAM_PLAYERS) 
                    throw new InvalidTeamException($"team must have min {MIN_TEAM_PLAYERS} players");

                if (data.Players.Length > MAX_TEAM_PLAYERS) 
                    throw new InvalidTeamException($"team must have max {MAX_TEAM_PLAYERS} players");

                foreach (var p in data.Players) {
                    if (_userContext.Items.Find(p) == null) 
                        throw new InvalidTeamException($"team player {p} doesn't exist");
                }
            }
        }
    }
}

public class InvalidTeamException : Exception {
    public InvalidTeamException(string message) : base(message) { }
}