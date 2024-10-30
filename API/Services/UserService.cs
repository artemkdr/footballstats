using API.Controllers;
using API.Data;
using API.Models;

namespace API.Services;

public interface IUserService 
{
    User? GetUser(string username);
    ListDTO GetUsers(string? username, string? status, int page, int limit);
    User CreateUser(UserDTO userData);
    User? UpdateUser(string username, UserDTO userData);
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

    public User? GetUser(string username)
    {
        return _userContext.Items.Find(username);
    }

    public ListDTO GetUsers(string? username, string? status, int page = 1, int limit = 0)
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
        
        var items = query.OrderBy(x => x.Username)
                            .Skip((page - 1) * limit)
                        .Take(limit)
                        .Select(x => new UserDTO() {
                            Username = x.Username,
                            Vars = x.Vars,                                
                            Status = x.Status.ToString()
                        })
                        .ToList();

        return new ListDTO {
            Page = page,
            PageSize = limit,
            Total = totalCount,
            TotalPages = totalPages,
            List = items.ToArray()
        };
    }

    public User CreateUser(UserDTO userData)
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
        _userContext.SaveChanges();

        // create a team with a single user automatically
        _teamContext.Items.Add(new Team {
            Name = newItem.Username,
            Players = new string[] { newItem.Username },
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = TeamStatus.Active
        });
        _teamContext.SaveChanges();

        return newItem;  
    }

    public User? UpdateUser(string username, UserDTO userData)
    {
        var item = _userContext.Items.Find(username);
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
            _userContext.SaveChanges();            
            return item;            
        }

        return null;
    }
}