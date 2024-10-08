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
    public class UserController_CreateUserTests
    {
        private readonly BaseDBContext<User> _userContext;
        private readonly BaseDBContext<Team> _teamContext;
        private readonly BaseDBContext<Game> _gameContext;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserController _controller;
        
        public UserController_CreateUserTests()
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
        public void CreatesUser_ReturnsCreatedAtActionResult() {
            string username = Guid.NewGuid().ToString();

            var result = _controller.CreateUser(new UserDTOFull {
                Username = username                
            });
            
            // Assert
            var resp = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<User>(resp.Value);

            // returns the same username 
            Assert.Equal(username, returnedUser.Username);
        }

        [Fact]
        public void CreatesUser_CreatesTeamWithUser() {
            string username = Guid.NewGuid().ToString();

            var result = _controller.CreateUser(new UserDTOFull {
                Username = username                
            });
            
            // checks that the team exists
            var team = _teamContext.Items.Single(x => x.Name == username);
            // team exists
            Assert.NotNull(team);            
            // team contains only one player and it's just created user
            Assert.True(team!.Players?.Length == 1 && team!.Players?[0] == username);
        }

        [Fact]
        public void CreateUser_EmptyUsername_ReturnsInternalServerErrorResult()
        {
            // Arrange 
            var username = Guid.NewGuid().ToString();
            _userContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );

            _userContext.SaveChanges();
            // Act
            var result = _controller.CreateUser(new UserDTOFull {
                Username = username,
                Status = UserStatus.Active.ToString()
            });

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);            
            Assert.NotNull(jsonResult.Value);
            object? errorValue = null;
            (jsonResult.Value as Dictionary<string,object>)?.TryGetValue("error", out errorValue);
            Assert.NotNull(errorValue);
        }

        [Fact]
        public void CreateUser_ExistingUsername_ReturnsInternalServerErrorResult()
        {
            // Act
            var result = _controller.CreateUser(new UserDTOFull {
                Status = UserStatus.Active.ToString()
            });

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