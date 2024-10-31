using System.Net;
using API.Controllers;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Controllers
{
    public class TeamController_CreateTeamTests : IDisposable
    {   
        private ControllersTests controllerTests;

        public TeamController_CreateTeamTests()
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
        public async void CreatesTeam_ReturnsCreatedAtActionResult() {
            // Arrange
            // create players
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            await controllerTests.UserContext.SaveChangesAsync();
            
            string name = Guid.NewGuid().ToString();
            var result = await controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = name,
                Players = new string[] { p1.Username }
            });
            
            // Assert
            var resp = Assert.IsType<CreatedAtActionResult>(result);
            var returnedTeam = Assert.IsType<Team>(resp.Value);

            // returns the same name 
            Assert.Equal(name, returnedTeam.Name);
            // default status is active
            Assert.Equal(TeamStatus.Active, returnedTeam.Status);
            // created team has players
            Assert.Equal(returnedTeam.Players, new string[] { p1.Username });
        }

        [DockerRequiredFact]
        public async void CreateTeam_EmptyName_ReturnsBadRequestResult()
        {
            // Arrange            
            // create players
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            await controllerTests.UserContext.SaveChangesAsync();

            // Act
            // send with existing players but without name
            var result = await controllerTests.TeamController.CreateTeam(new TeamDTO {                
                Status = TeamStatus.Active.ToString(),
                Players = new string[] { p1.Username }
            });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }

        [DockerRequiredFact]
        public async void CreateTeam_ExistingName_ReturnsInternalServerErrorResult()
        {
            // Arrange
            // create a team            
            var team = new Team { Name = Guid.NewGuid().ToString() };
            controllerTests.TeamContext.Items.Add(team);
            await controllerTests.TeamContext.SaveChangesAsync();

            // create players
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            await controllerTests.UserContext.SaveChangesAsync();
            
            // Act
            // send valid TeamDTO (name and existing players)
            // but the name already exists
            await Assert.ThrowsAsync<DbUpdateException>(async () => {
                await controllerTests.TeamController.CreateTeam(new TeamDTO {
                    Name = team.Name,
                    Players = new string[] { p1.Username }
                });
            });
        }

        [DockerRequiredFact]
        public async void CreateTeam_NoPlayers_ReturnsBadRequestResult()
        {
            // Act
            // send invalid TeamDTO - no name            
            var result = await controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString()
            });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }


        [DockerRequiredFact]
        public async void CreateTeam_NonExistingPlayers_ReturnsBadRequestResult()
        {
            // Act
            // send valid TeamDTO, but players don't exist in the database
            var result = await controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString(),
                Players = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }  

        [DockerRequiredFact]
        public async void CreateTeam_WrongMaxPlayersNumber_ReturnsBadRequestResult()
        {
            // Arrange
            // create players
            var players = new List<User>();
            for (var i = 0; i <= TeamService.MAX_TEAM_PLAYERS; i++) {
                players.Add(new User { Username = Guid.NewGuid().ToString() });
            }            
            controllerTests.UserContext.Items.AddRange(players);
            await controllerTests.UserContext.SaveChangesAsync();

            // Act
            // send valid TeamDTO, but players number exceeds the limit
            var result = await controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString(),
                Players = players.Select(x => x.Username!).ToArray()
            });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        } 

        [DockerRequiredFact]
        public async void CreateTeam_WrongMinPlayersNumber_ReturnsBadRequestResult()
        {
            // Arrange
            // create players
            var players = new List<User>();
            for (var i = 0; i < TeamService.MIN_TEAM_PLAYERS - 1; i++) {
                players.Add(new User { Username = Guid.NewGuid().ToString() });
            }
            controllerTests.UserContext.Items.AddRange(players);
            await controllerTests.UserContext.SaveChangesAsync();

            // Act
            // send valid TeamDTO, but players number exceeds the limit
            var result = await controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString(),
                Players = players.Select(x => x.Username!).ToArray()
            });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }
    }

    
}