using System.Net;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Controllers
{
    public class UserController_CreateUserTests : IDisposable
    {   
        private ControllersTests controllerTests;

        public UserController_CreateUserTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.UserContext.Database.BeginTransaction(); 
            controllerTests.TeamContext.Database.BeginTransaction(); // as UserController.CreateUser will also affect TeamContext
        }

        public void Dispose() {
            controllerTests.UserContext.Database.RollbackTransaction();
            controllerTests.TeamContext.Database.RollbackTransaction();  // as UserController.CreateUser also affected TeamContext
        }

        [DockerRequiredFact]        
        public async void CreatesUser_ReturnsCreatedAtActionResult() {
            string username = Guid.NewGuid().ToString();

            var result = await controllerTests.UserController.CreateUser(new UserDTO {
                Username = username                
            });
            
            // Assert
            var resp = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<User>(resp.Value);

            // returns the same username 
            Assert.Equal(username, returnedUser.Username);
        }

        [DockerRequiredFact]
        public async void CreatesUser_CreatesTeamWithUser() {
            string username = Guid.NewGuid().ToString();

            var result = await controllerTests.UserController.CreateUser(new UserDTO {
                Username = username                
            });
            
            // checks that the team exists
            var team = await controllerTests.TeamContext.Items.SingleAsync(x => x.Name == username);

            // team exists
            Assert.NotNull(team);            
            // team contains only one player and it's just created user
            Assert.True(team!.Players?.Length == 1 && team!.Players?[0] == username);
        }

        [DockerRequiredFact]
        public async void CreateUser_ExistingUsername_ReturnsInternalServerErrorResult()
        {
            // Arrange 
            var username = Guid.NewGuid().ToString();
            controllerTests.UserContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            await controllerTests.UserContext.SaveChangesAsync();

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await controllerTests.UserController.CreateUser(new UserDTO {
                    Username = username,
                    Status = UserStatus.Active.ToString()
                });
            });
        }

        [DockerRequiredFact]
        public async void CreateUser_EmptyUsername_ReturnsBadRequestResult()
        {
            // Act
            var result = await controllerTests.UserController.CreateUser(new UserDTO {
                Status = UserStatus.Active.ToString()
            });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }  
    }

    
}