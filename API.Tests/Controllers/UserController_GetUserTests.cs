using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class UserController_GetUserTests : IDisposable
    {
        private ControllersTests controllerTests;
        
        public UserController_GetUserTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.UserContext.Database.BeginTransaction();
        }

        public void Dispose() {
            controllerTests.UserContext.Database.RollbackTransaction();            
        }

            

        [DockerRequiredFact]
        public void GetUser_ExistingUser_ReturnsOkResultWithUserData()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var user = new User { 
                Username = username, 
                Status = UserStatus.Active, 
                Vars = JsonDocument.Parse("{ \"key1\": \"value\", \"key2\": 2 }") 
            };
            controllerTests.UserContext.Items.Add(user);
            controllerTests.UserContext.SaveChanges();

            // Act
            var result = controllerTests.UserController.GetUser(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDTO>(okResult.Value);

            Assert.Equal(username, returnedUser.Username);
            Assert.Equal(UserStatus.Active.ToString(), returnedUser.Status);
            Assert.Equal(user.Vars, returnedUser.Vars); 
        }

        [DockerRequiredFact]
        public void GetUser_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var user = new User { Username = username };
            
            // Act
            var result = controllerTests.UserController.GetUser(username);

            // Assert
            var typedResult = Assert.IsType<NotFoundObjectResult>(result);                    
            Assert.NotNull(typedResult.Value);                        
            Assert.NotNull((typedResult.Value as ErrorDTO)?.Error ?? (typedResult.Value as ErrorDTO)?.Detail);
        }
           
    }

    
}