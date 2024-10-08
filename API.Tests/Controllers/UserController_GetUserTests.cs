using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace API.Tests.Controllers
{
    public class UserController_GetUserTests
    {
        private readonly BaseDBContext<User> _userContext;
        private readonly BaseDBContext<Team> _teamContext;
        private readonly BaseDBContext<Game> _gameContext;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserController _controller;
        
        public UserController_GetUserTests()
        {  
            var optionsUser = new DbContextOptionsBuilder<BaseDBContext<User>>().UseNpgsql(TestContainers.GetConnectionString()).Options;
            var optionsTeam = new DbContextOptionsBuilder<BaseDBContext<Team>>().UseNpgsql(TestContainers.GetConnectionString()).Options;
            var optionsGame = new DbContextOptionsBuilder<BaseDBContext<Game>>().UseNpgsql(TestContainers.GetConnectionString()).Options;

            _userContext = new BaseDBContext<User>(optionsUser); 
            _teamContext = new BaseDBContext<Team>(optionsTeam); 
            _gameContext = new BaseDBContext<Game>(optionsGame); 
            _configurationMock = new Mock<IConfiguration>();
            _controller = new UserController(_configurationMock.Object, _userContext, _teamContext, _gameContext);
        }

        [Fact]
        public void GetUser_ExistingUser_ReturnsOkResultWithUserData()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var user = new User { 
                Username = username, 
                Status = UserStatus.Active, 
                Vars = JsonDocument.Parse("{ \"key1\": \"value\", \"key2\": 2 }") 
            };
            _userContext.Items.Add(user);
            _userContext.SaveChanges();

            // Act
            var result = _controller.GetUser(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDTO>(okResult.Value);

            Assert.Equal(username, returnedUser.Username);
            Assert.Equal(UserStatus.Active.ToString(), returnedUser.Status);
            Assert.Equal(user.Vars, returnedUser.Vars); 
        }

        [Fact]
        public void GetUser_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var user = new User { Username = username };
            
            // Act
            var result = _controller.GetUser(username);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, jsonResult.StatusCode);            
            Assert.NotNull(jsonResult.Value);
            object? errorValue = null;
            (jsonResult.Value as Dictionary<string,object>)?.TryGetValue("error", out errorValue);
            Assert.NotNull(errorValue);            
        }
           
    }

    
}