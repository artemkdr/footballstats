using System.Net;
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
        public async void UpdateTeam_ExistingTeam_ReturnsOkResult()
        {            
            // Arrange
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            controllerTests.UserContext.SaveChanges();
            
            string name = Guid.NewGuid().ToString();
            CreatedAtActionResult? teamResult = await controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = name,
                Players = new string[] { p1.Username }
            }) as CreatedAtActionResult;
            int id = (teamResult?.Value as Team)!.Id;

            // Act
            var result = await controllerTests.TeamController.UpdateTeam(id, new TeamDTO() {
                Status = TeamStatus.Deleted.ToString()                
            }); 

            var updatedTeam = await controllerTests.TeamContext.Items.FindAsync(id);

            // Assert
            var typedResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(TeamStatus.Deleted, updatedTeam?.Status);            
        }

        [DockerRequiredFact]
        public async void UpdateTeam_NonExistingTeam_ReturnsNotFoundResult()
        {
            // Act
            var result = await controllerTests.TeamController.UpdateTeam(Guid.NewGuid().ToString().GetHashCode(), new TeamDTO()); 

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }  
    }

    
}