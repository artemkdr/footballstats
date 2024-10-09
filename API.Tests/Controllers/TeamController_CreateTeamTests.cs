using System.Net;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

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
        public void CreatesTeam_ReturnsCreatedAtActionResult() {
            // Arrange
            // create players
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            controllerTests.UserContext.SaveChanges();
            
            string name = Guid.NewGuid().ToString();
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {
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
        public void CreateTeam_EmptyName_ReturnsInternalServerErrorResult()
        {
            // Arrange            
            // create players
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            controllerTests.UserContext.SaveChanges();

            // Act
            // send with existing players but without name
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {                
                Status = TeamStatus.Active.ToString(),
                Players = new string[] { p1.Username }
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
        public void CreateTeam_ExistingName_ReturnsInternalServerErrorResult()
        {
            // Arrange
            // create a team            
            var team = new Team { Name = Guid.NewGuid().ToString() };
            controllerTests.TeamContext.Items.Add(team);
            controllerTests.TeamContext.SaveChanges();

            // create players
            var p1 = new User { Username = Guid.NewGuid().ToString() };
            controllerTests.UserContext.Items.Add(p1);
            controllerTests.UserContext.SaveChanges();
            
            // Act
            // send valid TeamDTO (name and existing players)
            // but the name already exists
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = team.Name,
                Players = new string[] { p1.Username }
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
        public void CreateTeam_NoPlayers_ReturnsInternalServerErrorResult()
        {
            // Act
            // send invalid TeamDTO - no name            
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString()
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
        public void CreateTeam_NonExistingPlayers_ReturnsInternalServerErrorResult()
        {
            // Act
            // send valid TeamDTO, but players don't exist in the database
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString(),
                Players = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
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
        public void CreateTeam_WrongMaxPlayersNumber_ReturnsInternalServerErrorResult()
        {
            // Arrange
            // create players
            var players = new List<User>();
            for (var i = 0; i <= BaseController.MAX_TEAM_PLAYERS; i++) {
                players.Add(new User { Username = Guid.NewGuid().ToString() });
            }            
            controllerTests.UserContext.Items.AddRange(players);
            controllerTests.UserContext.SaveChanges();

            // Act
            // send valid TeamDTO, but players number exceeds the limit
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString(),
                Players = players.Select(x => x.Username!).ToArray()
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
        public void CreateTeam_WrongMinPlayersNumber_ReturnsInternalServerErrorResult()
        {
            // Arrange
            // create players
            var players = new List<User>();
            for (var i = 0; i < BaseController.MIN_TEAM_PLAYERS - 1; i++) {
                players.Add(new User { Username = Guid.NewGuid().ToString() });
            }
            controllerTests.UserContext.Items.AddRange(players);
            controllerTests.UserContext.SaveChanges();

            // Act
            // send valid TeamDTO, but players number exceeds the limit
            var result = controllerTests.TeamController.CreateTeam(new TeamDTO {
                Name = Guid.NewGuid().ToString(),
                Players = players.Select(x => x.Username!).ToArray()
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