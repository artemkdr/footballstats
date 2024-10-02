using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers;

public class UserModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }    
}

[ApiController]
public class UserController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;
    
    public UserController(IConfiguration configuration)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
    }

    [Route("user/{username}")]    
    [HttpGet]
    public IActionResult GetUser(string username)
    {
        return RequestHelpers.Success(RequestHelpers.ToDict("username", username));
    }

    [Route("user")]
    [HttpPost]
    public IActionResult CreateUser(UserModel userData)
    {
        if (userData != null && userData.Username != null) {
            return RequestHelpers.Success(RequestHelpers.ToDict("username", userData.Username));
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "username is missing"));
    }

    [Route("user/{username}")]
    [HttpPost]
    public IActionResult UpdateUser(string username, UserModel userData)
    {
        if (userData != null && userData.Username != null) {
            return RequestHelpers.Success(RequestHelpers.ToDict("username", userData.Username));
        }
        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "username is missing"));
    }
}