using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

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
        public void UpdateUser_ExistingUser_ReturnsOkResult()
        {            
            // Arrange
            var username = Guid.NewGuid().ToString();
            controllerTests.UserContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            controllerTests.UserContext.SaveChanges();

            // Act
            var result = controllerTests.UserController.UpdateUser(username, new UserDTOFull() {
                Status = UserStatus.Deleted.ToString(),
                Vars = JsonDocument.Parse("{ \"key1\": \"value\", \"key2\": 2 }") 
            }); 

            // Assert
            var typedResult = Assert.IsType<OkObjectResult>(result);
            
            var updatedUser = controllerTests.UserContext.Items.Single(x => x.Username == username);
            Assert.Equal(UserStatus.Deleted, updatedUser.Status);
            Assert.NotNull(updatedUser.Vars);            
            JsonElement el = new JsonElement();
            Assert.True(updatedUser.Vars?.RootElement.TryGetProperty("key2", out el));            
            Assert.Equal(2, el.GetInt16());
        }

        [DockerRequiredFact]
        public void UpdateUser_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();

            // Act
            var result = controllerTests.UserController.UpdateUser(username, new UserDTOFull()); 

            // Assert
            var typedResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull((typedResult.Value as ErrorDTO)?.Error ?? (typedResult.Value as ErrorDTO)?.Detail);
        }

        [DockerRequiredFact]
        public void UpdateUser_UpdateUsername_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            controllerTests.UserContext.Items.AddRange(
                new User { Username = username, Status = UserStatus.Active }
            );
            controllerTests.UserContext.SaveChanges();

            // Act
            var result = controllerTests.UserController.UpdateUser(username, new UserDTOFull { Username = "newusername" }); 

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);       
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);                     
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }
           
    }

    
}