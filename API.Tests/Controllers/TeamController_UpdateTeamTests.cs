using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class TeamController_UpdateTeamTests : IDisposable
    {
        private ControllersTests controllerTests;

        public TeamController_UpdateTeamTests()
        {
            controllerTests = new ControllersTests();
            controllerTests.TeamContext.Database.BeginTransaction();
            controllerTests.UserContext.Database.BeginTransaction(); // as we will create Users for the team creation
        }

        public void Dispose() {
            controllerTests.TeamContext.Database.RollbackTransaction();  
            controllerTests.UserContext.Database.RollbackTransaction(); // as we will created Users for the team creation          
        }

        [DockerRequiredFact]
        public void UpdateTeam_ExistingTeam_ReturnsOkResult()
        {            
            // Arrange
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            controllerTests.UserContext.SaveChanges();
            
            string name = Guid.NewGuid().ToString();
            CreatedAtActionResult? teamResult = controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = name,
                Players = new string[] { p1.Username }
            }) as CreatedAtActionResult;
            int id = (teamResult?.Value as Team)!.Id;

            // Act
            var result = controllerTests.TeamController.UpdateTeam(id, new TeamDTO() {
                Status = TeamStatus.Deleted.ToString()                
            }); 

            var updatedTeam = controllerTests.TeamContext.Items.Find(id);

            // Assert
            var jsonResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(TeamStatus.Deleted, updatedTeam?.Status);            
        }

        [DockerRequiredFact]
        public void UpdateTeam_NonExistingTeam_ReturnsNotFoundResult()
        {
            // Act
            var result = controllerTests.TeamController.UpdateTeam(Guid.NewGuid().ToString().GetHashCode(), new TeamDTO()); 

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