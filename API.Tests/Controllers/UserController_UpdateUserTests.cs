using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Controllers
{
    public class UserController_UpdateUserTests : IDisposable
    {
        private ControllersTests controllerTests;

        public UserController_UpdateUserTests()
        {
            controllerTests = new ControllersTests();
            controllerTests.UserContext.Database.BeginTransaction();
        }

        public void Dispose() {
            controllerTests.UserContext.Database.RollbackTransaction();            
        }

        [DockerRequiredFact]
        public async void UpdateUser_ExistingUser_ReturnsOkResult()
        {            
            // Arrange
            var username = Guid.NewGuid().ToString();
            controllerTests.UserContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            await controllerTests.UserContext.SaveChangesAsync();

            // Act
            var result = await controllerTests.UserController.UpdateUser(username, new UserDTO() {
                Status = UserStatus.Deleted.ToString(),
                Vars = JsonDocument.Parse("{ \"key1\": \"value\", \"key2\": 2 }") 
            }); 

            // Assert
            var typedResult = Assert.IsType<OkObjectResult>(result);
            
            var updatedUser = await controllerTests.UserContext.Items.SingleAsync(x => x.Username == username);
            Assert.Equal(UserStatus.Deleted, updatedUser.Status);
            Assert.NotNull(updatedUser.Vars);            
            JsonElement el = new JsonElement();
            Assert.True(updatedUser.Vars?.RootElement.TryGetProperty("key2", out el));            
            Assert.Equal(2, el.GetInt16());
        }

        [DockerRequiredFact]
        public async void UpdateUser_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();

            // Act
            var result = await controllerTests.UserController.UpdateUser(username, new UserDTO()); 

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }

        [DockerRequiredFact]
        public async void UpdateUser_UpdateUsername_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            controllerTests.UserContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            await controllerTests.UserContext.SaveChangesAsync();

            // Act
            var result = await controllerTests.UserController.UpdateUser(username, new UserDTO { Username = "newusername" }); 

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);       
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);                     
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }
           
    }

    
}