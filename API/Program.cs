using Microsoft.OpenApi.Models;
using NLog.Web;
using NLog.Extensions.Logging;
using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddNLog();


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(",", StringSplitOptions.RemoveEmptyEntries);        
        if (allowedOrigins?.Any() == true) {            
            corsBuilder.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddDbContext<BaseDBContext<User>>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))  
    .AddDbContext<BaseDBContext<Game>>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddDbContext<BaseDBContext<Team>>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));   

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ITeamService, TeamService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
var swaggerUrlPrefix = builder.Configuration.GetValue<string>("SwaggerUrlPrefix");
app.UseSwagger(c =>
{
    if (swaggerUrlPrefix != null)
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                paths.Add((swaggerUrlPrefix + path.Key).Replace("//", "/"), path.Value);
            }
            swaggerDoc.Paths = paths;
        });
    }    
});
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();
app.MapControllers();

// exceptions handler
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        var problemDetails = new ProblemDetails() {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        if (app.Environment.IsDevelopment()) {
            problemDetails.Detail = exception?.Message;
            problemDetails.Extensions["stackTrace"] = exception?.StackTrace;
        }

        NLog.LogManager.GetCurrentClassLogger().Error(exception);

        context.Response.StatusCode = problemDetails.Status.Value;
        context.Response.ContentType = "application/problem+json";   

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.Run();
