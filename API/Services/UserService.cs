using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public interface IUserService 
{
    Task<User?> GetUserAsync(string username);
    Task<PagedList<User>> GetUsersAsync(string? username, string? status, int page, int limit);
    Task<List<User>> GetUsersAsync(string[] usernames);
    Task<User> CreateUserAsync(UserDTO userData);
    Task<User?> UpdateUserAsync(string username, UserDTO userData);
}

public class UserService : IUserService {

    public static readonly int LIST_LIMIT = 500;
    private readonly BaseDBContext<User> _userContext;
    private readonly BaseDBContext<Team> _teamContext; 

    public UserService(BaseDBContext<User> userContext, BaseDBContext<Team> teamContext)
    {
        _userContext = userContext;
        _teamContext = teamContext;
    }

    public async Task<User?> GetUserAsync(string username)
    {
        return await _userContext.Items.FindAsync(username);
    }

    public async Task<PagedList<User>> GetUsersAsync(string? username, string? status, int page = 1, int limit = 0)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = Math.Max(limit, UserService.LIST_LIMIT);

        var query = _userContext.Items.AsQueryable();

        // case insensitive "like '%value%'" search by name
        if (!string.IsNullOrEmpty(username))
            query = query.Where(x => x.Username != null && x.Username.ToLower().Contains(username.ToLower()));

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<UserStatus>(status, true, out var statusEnum))
            query = query.Where(x => x.Status == statusEnum);            
            
        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / limit);
        if (page > totalPages) page = Math.Max(1, totalPages);
        
        var items = await query.OrderBy(x => x.Username)
                            .Skip((page - 1) * limit)
                        .Take(limit)                        
                        .ToListAsync();

        return new PagedList<User> {
            Page = page,
            PageSize = limit,
            Total = totalCount,
            TotalPages = totalPages,
            List = items
        };
    }

    public async Task<List<User>> GetUsersAsync(string[] usernames) {
        var query = _userContext.Items.AsQueryable();
        query = query.Where(x => usernames.Contains(x.Username));
        return await query.ToListAsync(); 
    }

    public async Task<User> CreateUserAsync(UserDTO userData)
    {
        if (userData == null || userData.Username == null) {
            throw new ArgumentNullException("userData or userData.Username is missing");
        }
        // TODO: add data validation            
        // TODO: check that the username is a valid name
        // TODO: check the password is valid and hash password

        var newItem = new User {
            Username = userData.Username,
            Password = userData.Password, 
            Vars = userData.Vars,
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = UserStatus.Active            
        };
        if (userData.Status != null && Enum.TryParse<UserStatus>(userData.Status, out var status))
            newItem.Status = status;
        
        _userContext.Items.Add(newItem);
        await _userContext.SaveChangesAsync();

        // create a team with a single user automatically
        _teamContext.Items.Add(new Team {
            Name = newItem.Username,
            Players = new string[] { newItem.Username },
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = TeamStatus.Active
        });
        await _teamContext.SaveChangesAsync();
        return newItem;  
    }

    public async Task<User?> UpdateUserAsync(string username, UserDTO userData)
    {
        var item = await _userContext.Items.FindAsync(username);
        if (item == null)            
            return null;
        
        if (userData != null) {
            // TODO: add data validation            
            // TODO: check that the username is a valid name            
            // TODO: check the password is valid and hash password
            
            if (userData.Username != null && userData.Username != item.Username) {                
                throw new InvalidOperationException("Username cannot be updated");
            }

            if (userData.Password != null)
                item.Password = userData.Password;
            if (userData.Vars != null) 
                item.Vars = userData.Vars;
            
            if (userData.Status != null && Enum.TryParse<UserStatus>(userData.Status, out var status))
                item.Status = status;

            item.ModifyDate = DateTime.UtcNow; 
            
            _userContext.Items.Update(item);
            await _userContext.SaveChangesAsync();            
            return item;            
        }

        return null;
    }
}