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

public class UserDTOLight : IBaseDTO
{
    public string? Username { get; set; }
}

public class UserDTO : UserDTOLight
{   
    public JsonDocument? Vars { get; set;}
    
    public string? Status { get; set; }
}

public class UserDTOFull : UserDTO 
{
    public string? Password { get; set; }
}

[ApiController]
public class UserController : BaseController
{
    public UserController(IConfiguration configuration, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext, BaseDBContext<Game> gameContext) 
        : base(configuration, userContext, teamContext, gameContext)
    {
    }

    [Route("user/{username}")]    
    [HttpGet]
    public IActionResult GetUser(string username)
    {
        try {
            var item = _userContext.Items.Find(username);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"User '{username}' not found"), Response, (int)HttpStatusCode.NotFound);
            return Ok(new UserDTO() {
               Username = item.Username,
               Vars = item.Vars,
               Status = item.Status.ToString()
            });
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("user")]
    [HttpGet]
    public IActionResult GetUsers(string? username = null, string? status = null, int page = 1) 
    {
        if (page < 1) page = 1;
        try {
            var query = _userContext.Items.AsQueryable();

            // case insensitive "like '%value%'" search by name
            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Username != null && x.Username.ToLower().Contains(username.ToLower()));

            if (!string.IsNullOrEmpty(status))
                if (Enum.TryParse<UserStatus>(status, true, out var statusEnum))
                    query = query.Where(x => x.Status == statusEnum);
                else 
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"status '{status}' doesn't exist"));                
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / LIST_LIMIT);
            if (page > totalPages) page = Math.Max(1, totalPages);
            
            var items = query.OrderBy(x => x.Username)
                             .Skip((page - 1) * LIST_LIMIT)
                            .Take(LIST_LIMIT)
                            .Select(x => new UserDTO() {
                                Username = x.Username,
                                Vars = x.Vars,                                
                                Status = x.Status.ToString()
                            })
                            .ToList();

            return Ok(new ListDTO {
                Page = page,
                PageSize = LIST_LIMIT,
                Total = totalCount,
                TotalPages = totalPages,
                List = items.ToArray()
            });
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("user")]
    [HttpPost]
    public IActionResult CreateUser(UserDTOFull userData)
    {
        if (userData == null || userData.Username == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", "username is missing"));
        
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

        try {
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

            return CreatedAtAction(nameof(GetUser), new { username = newItem.Username }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("user/{username}")]
    [HttpPost]
    public IActionResult UpdateUser(string username, UserDTOFull userData)
    {        
        var item = _userContext.Items.Find(username);
        if (item == null)
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"User '{username}' not found"), Response, (int)HttpStatusCode.NotFound);
        
        if (userData != null) {
            // TODO: add data validation            
            // TODO: check that the username is a valid name            
            // TODO: check the password is valid and hash password
            
            if (userData.Username != null && userData.Username != item.Username) {                
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", "username cannot be updated"));                
            }

            if (userData.Password != null)
                item.Password = userData.Password;
            if (userData.Vars != null) 
                item.Vars = userData.Vars;
            
            if (userData.Status != null && Enum.TryParse<UserStatus>(userData.Status, out var status))
                item.Status = status;

            item.ModifyDate = DateTime.UtcNow; 
            try {
                _userContext.Items.Update(item);
                _userContext.SaveChanges();            
                var dto = new UserDTOLight() {
                    Username = item.Username
                };
                return Ok(dto);
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
            }
        }

        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no user data provided"));
    }
}