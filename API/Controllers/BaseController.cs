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

[ApiController]
abstract public class BaseController : ControllerBase
{
    public static int LIST_LIMIT = 1000;

    public static int MAX_TEAM_PLAYERS = 2;
    public static int MIN_TEAM_PLAYERS = 1;
    
    protected readonly IConfiguration _configuration;
    
    protected readonly Logger _logger;

    protected readonly BaseDBContext<User> _userContext;

    protected readonly BaseDBContext<Team> _teamContext;

    protected readonly BaseDBContext<Game> _gameContext;
    
    public BaseController(IConfiguration configuration, BaseDBContext<User> userContext, BaseDBContext<Team> teamContext, BaseDBContext<Game> gameContext)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
        _userContext = userContext;
        _teamContext = teamContext;
        _gameContext = gameContext;
    }
}