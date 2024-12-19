using System.Net;
using System.Text.Json;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UserDTO : IBaseDTO
{
    public string? Username { get; set; }

    public JsonDocument? Vars { get; set;}
    
    public string? Status { get; set; }

    public string? Password { get; set; }
}


[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _userService; // Inject the service

    public UserController(
            IConfiguration configuration, 
            BaseDBContext<User> userContext, 
            BaseDBContext<Team> teamContext, 
            BaseDBContext<Game> gameContext, 
            IUserService userService) 
        : base(configuration, userContext, teamContext, gameContext)
    {
        _userService = userService;
    }

    [Route("users/{username}")]    
    [HttpGet]
    public async Task<IActionResult> GetUser(string username)
    {        
        var item = await _userService.GetUserAsync(username);
        if (item == null)
            return NotFoundProblem($"User '{username}' not found");
        return Ok(new UserDTO() {
            Username = item.Username,
            Vars = item.Vars,
            Status = item.Status.ToString()
        });        
    }
    

    [Route("users")]
    [HttpGet]
    public async Task<IActionResult> GetUsers(string? username = null, string? status = null, int page = 1, int limit = 0) 
    {
        if (!string.IsNullOrEmpty(status) && !Enum.TryParse<UserStatus>(status, true, out var statusEnum))
            return BadRequestProblem($"status '{status}' doesn't exist");

        var usersPages = await _userService.GetUsersAsync(username, status, page, limit);
        
        return Ok(new ListDTO() {
            List = usersPages.List?.Select(x => new UserDTO() {
                Username = x.Username,
                Status = x.Status.ToString()
            }).ToArray(),
            Page = usersPages.Page,
            PageSize = usersPages.PageSize,
            Total = usersPages.Total,
            TotalPages = usersPages.TotalPages
        });
    }

    [Route("users")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDTO userData)
    {
        if (userData == null || userData.Username == null)
            return BadRequestProblem("username is missing");
        
        var newItem = await _userService.CreateUserAsync(userData);
        return CreatedAtAction(nameof(GetUser), new { username = newItem.Username }, newItem);        
    }

    [Route("users/{username}")]
    [HttpPatch]
    public async Task<IActionResult> UpdateUser(string username, UserDTO userData)
    {   
        if (userData == null)
            return BadRequestProblem("no user data provided");

        try {
            var user = await _userService.UpdateUserAsync(username, userData);
            if (user == null) {
                return NotFoundProblem($"User '{username}' not found");                
            }
            var dto = new UserDTO() {
                Username = user.Username
            };
            return Ok(dto);
        } catch (InvalidOperationException) {
            return Problem("username cannot be updated");
        } catch (Exception ex) {
            return Problem(ex.InnerException?.Message ?? ex.Message);
        }
    }
}