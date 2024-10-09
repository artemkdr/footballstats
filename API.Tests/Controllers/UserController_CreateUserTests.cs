using System.Net;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class UserController_CreateUserTests : IDisposable
    {   
        private ControllersTests controllerTests;

        public UserController_CreateUserTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.UserContext.Database.BeginTransaction();
        }

        public void Dispose() {
            controllerTests.UserContext.Database.RollbackTransaction();            
        }

        [DockerRequiredFact]        
        public void CreatesUser_ReturnsCreatedAtActionResult() {
            string username = Guid.NewGuid().ToString();

            var result = controllerTests.UserController.CreateUser(new UserDTOFull {
                Username = username                
            });
            
            // Assert
            var resp = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<User>(resp.Value);

            // returns the same username 
            Assert.Equal(username, returnedUser.Username);
        }

        [DockerRequiredFact]
        public void CreatesUser_CreatesTeamWithUser() {
            string username = Guid.NewGuid().ToString();

            var result = controllerTests.UserController.CreateUser(new UserDTOFull {
                Username = username                
            });
            
            // checks that the team exists
            var team = controllerTests.TeamContext.Items.Single(x => x.Name == username);

            // team exists
            Assert.NotNull(team);            
            // team contains only one player and it's just created user
            Assert.True(team!.Players?.Length == 1 && team!.Players?[0] == username);
        }

        [DockerRequiredFact]
        public void CreateUser_ExistingUsername_ReturnsInternalServerErrorResult()
        {
            // Arrange 
            var username = Guid.NewGuid().ToString();
            controllerTests.UserContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            controllerTests.UserContext.SaveChanges();
            
            // Act
            var result = controllerTests.UserController.CreateUser(new UserDTOFull {
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

        [DockerRequiredFact]
        public void CreateUser_EmptyUsername_ReturnsInternalServerErrorResult()
        {
            // Act
            var result = controllerTests.UserController.CreateUser(new UserDTOFull {
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