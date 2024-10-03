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
    
    public string? Status { get; set; }
}

[ApiController]
public class UserController : BaseController<User>
{
    public UserController(IConfiguration configuration, BaseDBContext<User> context) 
        : base(configuration, context) 
    {
    }

    [Route("user/{username}")]    
    [HttpGet]
    public IActionResult GetUser(string username)
    {
        try {
            var item = _context.Items.Find(username);
            if (item == null)
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"User '{username}' not found"), Response, (int)HttpStatusCode.NotFound);
            return RequestHelpers.Success(RequestHelpers.ToDict("username", username));
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("user")]
    [HttpGet]
    public IActionResult GetUsers(string? username = null, string? status = null) 
    {
        try {
            var query = _context.Items.AsQueryable();

            // case insensitive "like '%value%'" search by name
            if (!string.IsNullOrEmpty(username))
                query = query.Where(x => x.Username != null && x.Username.ToLower().Contains(username.ToLower()));

            if (!string.IsNullOrEmpty(status))
                if (Enum.TryParse<UserStatus>(status, true, out var statusEnum))
                    query = query.Where(x => x.Status == statusEnum);
                else 
                    return RequestHelpers.Failure(RequestHelpers.ToDict("error", $"status '{status}' doesn't exist"));                
            
            var items = query.OrderBy(x => x.Username)
                            .Take(LIST_LIMIT)
                            .ToList();

            return Ok(items);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
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
            CreateDate = DateTime.UtcNow,            
            ModifyDate = DateTime.UtcNow,
            Status = UserStatus.Active            
        };
        if (userData.Status != null && Enum.TryParse<UserStatus>(userData.Status, out var status))
            newItem.Status = status;

        try {
            _context.Items.Add(newItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUser), new { username = newItem.Username }, newItem);
        } catch (Exception ex) {
            return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
        }
    }

    [Route("user/{username}")]
    [HttpPost]
    public IActionResult UpdateUser(string username, UserModel userData)
    {        
        var item = _context.Items.Find(username);
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
                _context.Items.Update(item);
                _context.SaveChanges();            
                return RequestHelpers.Success(RequestHelpers.ToDict("username", item.Username!));
            } catch (Exception ex) {
                return RequestHelpers.Failure(RequestHelpers.ToDict("error", ex.InnerException?.Message ?? ex.Message));
            }
        }

        return RequestHelpers.Failure(RequestHelpers.ToDict("error", "no user data provided"));
    }
}