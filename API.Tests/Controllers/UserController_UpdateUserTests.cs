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
    public class UserController_UpdateUserTests
    {
        private readonly BaseDBContext<User> _userContext;
        private readonly BaseDBContext<Team> _teamContext;
        private readonly BaseDBContext<Game> _gameContext;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserController _controller;
        
        public UserController_UpdateUserTests()
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
        public void UpdateUser_ExistingUser_ReturnsOkResult()
        {            
            // Arrange
            var username = Guid.NewGuid().ToString();
            _userContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            _userContext.SaveChanges();

            // Act
            var result = _controller.UpdateUser(username, new UserDTOFull() {
                Status = UserStatus.Deleted.ToString(),
                Vars = JsonDocument.Parse("{ \"key1\": \"value\", \"key2\": 2 }") 
            }); 

            // Assert
            var jsonResult = Assert.IsType<OkObjectResult>(result);
            
            var updatedUser = _userContext.Items.Single(x => x.Username == username);
            Assert.Equal(UserStatus.Deleted, updatedUser.Status);
            Assert.NotNull(updatedUser.Vars);            
            JsonElement el = new JsonElement();
            Assert.True(updatedUser.Vars?.RootElement.TryGetProperty("key2", out el));            
            Assert.Equal(2, el.GetInt16());
        }

        [Fact]
        public void UpdateUser_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();

            // Act
            var result = _controller.UpdateUser(username, new UserDTOFull()); 

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, jsonResult.StatusCode);            
            Assert.NotNull(jsonResult.Value);
            object? errorValue = null;
            (jsonResult.Value as Dictionary<string,object>)?.TryGetValue("error", out errorValue);
            Assert.NotNull(errorValue);
        }

        [Fact]
        public void UpdateUser_UpdateUsername_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            _userContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            _userContext.SaveChanges();

            // Act
            var result = _controller.UpdateUser(username, new UserDTOFull { Username = "newusername" }); 

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);            
            Assert.NotNull(jsonResult.Value);
            object? errorValue = null;
            (jsonResult.Value as Dictionary<string,object>)?.TryGetValue("error", out errorValue);
            Assert.NotNull(errorValue);
        }
           
    }

    
}