using System.Net;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class TeamController_GetTeamTests : IDisposable
    {
        private ControllersTests controllerTests;
        
        public TeamController_GetTeamTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.TeamContext.Database.BeginTransaction();
        }

        public void Dispose() {
            controllerTests.TeamContext.Database.RollbackTransaction();            
        }

        [DockerRequiredFact]
        public void GetTeam_ExistingTeam_ReturnsOkResultWithTeamData()
        {
            // Arrange
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            var p2 = new User { Username = Guid.NewGuid().ToString() };
            
            var teamname = Guid.NewGuid().ToString();
            var team = new Team { 
                Name = teamname, 
                Status = TeamStatus.Active,
                Players = new string[] { p1.Username, p2.Username }
            };
            controllerTests.TeamContext.Items.Add(team);
            controllerTests.TeamContext.SaveChanges();

            // Act
            var result = controllerTests.TeamController.GetTeam(team.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTeam = Assert.IsType<TeamDTOExtended>(okResult.Value);

            Assert.Equal(teamname, returnedTeam.Name);
            Assert.Equal(TeamStatus.Active.ToString(), returnedTeam.Status);
            Assert.IsType<UserDTO[]>(returnedTeam.Players);
            Assert.Equal(returnedTeam.Players?.Select(x => x.Username), new string[] { p1.Username, p2.Username }); 
        }

        [DockerRequiredFact]
        public void GetTeam_NonExistingTeamReturnsNotFoundResult()
        {
            // Arrange
            var id = Math.Abs(Guid.NewGuid().ToString().GetHashCode());

            // Act
            var result = controllerTests.TeamController.GetTeam(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }
           
    }

    
}