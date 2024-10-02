using Microsoft.OpenApi.Models;
using NLog.Web;
using NLog.Extensions.Logging;
using NLog;
using API.Data;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddDbContext<UserContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));   
builder.Services.AddDbContext<TeamContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));   
builder.Services.AddDbContext<GameContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));   


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

app.Run();
