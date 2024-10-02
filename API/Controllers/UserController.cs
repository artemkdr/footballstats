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

public class UserModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public JsonDocument? Vars { get; set;}
}

[ApiController]
public class UserController : ControllerBase
{
    protected readonly IConfiguration _configuration;
    private readonly Logger _logger;

    private readonly UserContext _context;
    
    public UserController(IConfiguration configuration, UserContext context)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
        _context = context;
    }

    [Route("user/{username}")]    
    [HttpGet]
    public IActionResult GetUser(string username)
    {
        try {
            var item = _context.Users.Find(username);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"User '{username}' not found"), Response, (int)HttpStatusCode.NotFound);
            return RequestHelpers.Success(RequestHelpers.ToDict("username", username));
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }
    }

    [Route("user")]
    [HttpPost]
    public IActionResult CreateUser(UserModel userData)
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
            Createdate = DateTime.UtcNow,            
            Modifydate = DateTime.UtcNow
        };

        try {
            _context.Users.Add(newItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUser), new { username = newItem.Username }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
        }
    }

    [Route("user/{username}")]
    [HttpPost]
    public IActionResult UpdateUser(string username, UserModel userData)
    {        
        var item = _context.Users.Find(username);
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
            
            item.Modifydate = DateTime.UtcNow; 
            try {
                _context.Users.Update(item);
                _context.SaveChanges();            
                return RequestHelpers.Success(RequestHelpers.ToDict("username", item.Username!));
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.Message));
            }
        }

        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no user data provided"));
    }
}