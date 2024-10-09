using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers;

[ApiController]
abstract public class BaseController : ControllerBase
{
    public static readonly int LIST_LIMIT = 500;

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

public interface IBaseDTO {

}

public class ListDTO : IBaseDTO {
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public object[]? List { get; set; }
}

public class ErrorDTO: IBaseDTO {
    public string Error { get; set; }

    public int? ErrorCode { get; set; }

    public string? Detail { get; set; }

    public ErrorDTO(string text, int? errorCode = null) {
        this.Error = text;
        this.ErrorCode = errorCode;
    }
}