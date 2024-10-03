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
abstract public class BaseController<T> : ControllerBase where T : class
{
    public static int LIST_LIMIT = 1000;
    
    protected readonly IConfiguration _configuration;
    
    protected readonly Logger _logger;

    protected readonly BaseDBContext<T> _context;
    
    public BaseController(IConfiguration configuration, BaseDBContext<T> context)
    {    
        _configuration = configuration;
        _logger = NLog.LogManager.GetCurrentClassLogger();
        _context = context;
    }
}