using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public interface IGameService 
{
    public Task<Game?> GetGameAsync(int id);
    public Task<PagedList<Game>> GetGamesAsync(int? team1, int? team2, DateTime? fromDate, DateTime? toDate, string? status, string? players, int page, int limit);
    public Task<Game> CreateGameAsync(GameDTO data);
    public Task<Game?> UpdateGameAsync(int id, GameDTO data);
}

public class GameService : IGameService {

    public static readonly int LIST_LIMIT = 500;
    
    private readonly BaseDBContext<User> _userContext;
    private readonly BaseDBContext<Team> _teamContext; 
    private readonly BaseDBContext<Game> _gameContext; 

    public GameService(BaseDBContext<Game> gameContext, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext)
    {
        _gameContext = gameContext;
        _userContext = userContext;
        _teamContext = teamContext;
    }

    public async Task<Game> CreateGameAsync(GameDTO data)
    {
        ValidateGameData(data, true);

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
        
        _gameContext.Items.Add(newItem);
        await _gameContext.SaveChangesAsync();
        return newItem;
    }

    public async Task<Game?> GetGameAsync(int id)
    {
        var item = await _gameContext.Items.FindAsync(id);
        return item;
    }

    public async Task<PagedList<Game>> GetGamesAsync(int? team1 = null, int? team2 = null, DateTime? fromDate = null, DateTime? toDate = null, string? status = null, string? players = null,
        int page = 1, int limit = 0)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = Math.Max(limit, GameService.LIST_LIMIT);
        
        var query = _gameContext.Items.AsQueryable();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<GameStatus>(status, true, out var statusEnum))
            query = query.Where(x => x.Status == statusEnum);
        
        if (players != null) {
            string[] playersArray = players.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var playersTeams = await _teamContext.Items
                .Where(x => x.Players != null && playersArray.All(p => x.Players.Contains(p)))
                .Select(x => x.Id)
                .ToListAsync();

            if (playersTeams != null && playersTeams.Count > 0) {
                query = query.Where(x => playersTeams.Contains(x.Team1) || playersTeams.Contains(x.Team2));
            }            
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

        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / limit);
        if (page > totalPages) page = Math.Max(1, totalPages);

        var items = await query                    
                .OrderBy(x => x.Status == GameStatus.Playing ? 0 : 
                                x.Status == GameStatus.Completed ? 1 :
                                x.Status == GameStatus.NotStarted ? 2 : 3) // Order by Status priority
                .ThenByDescending(x => x.Status == GameStatus.Playing ? x.ModifyDate :
                                        x.Status == GameStatus.Completed ? x.CompleteDate : x.ModifyDate)
                .Skip((page - 1) * limit)
                .Take(limit)                    
                .ToListAsync();
        
        return new PagedList<Game>() {
            List = items,
            Page = page,
            PageSize = limit,
            Total = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<Game?> UpdateGameAsync(int id, GameDTO data)
    {
        var item = await _gameContext.Items.FindAsync(id);
        if (item == null)
            return null;
        
        ValidateGameData(data);

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
            
            _gameContext.Items.Update(item);
            await _gameContext.SaveChangesAsync();            
        }
        return item;
    }

    private void ValidateGameData(GameDTO data, bool create = false) {
        if (data == null)
            throw new InvalidGameException("game data is empty");
        if (create) {
            if (data.Team1 == 0)
                throw new InvalidGameException("game team1 is missing");
            if (data.Team2 == 0)
                throw new InvalidGameException("game team2 is missing");
        }

        if (data.Team1 < 0)
            throw new InvalidGameException("game team1 is invalid");
        if (data.Team2 < 0)
            throw new InvalidGameException("game team2 is invalid");
        if (data.Team1 > 0 && data.Team2 > 0 && data.Team1 == data.Team2)
            throw new InvalidGameException("game team1 and team2 must be different");        
    }
}

public class InvalidGameException : Exception {
    public InvalidGameException(string message) : base(message) { }
}