using System.Net;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class GameController_CreateGameTests : IDisposable
    {   
        private ControllersTests controllerTests;

        public GameController_CreateGameTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.GameContext.Database.BeginTransaction();            
        }

        public void Dispose() {
            controllerTests.GameContext.Database.RollbackTransaction();                        
        }

        [DockerRequiredFact]        
        public void CreatesGame_ReturnsCreatedAtActionResult() {                        
            var result = controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1,
                Team2 = 2,
                Goals1 = 2,
                Goals2 = 5
            });
            
            // Assert
            var resp = Assert.IsType<CreatedAtActionResult>(result);
            var returnedGame = Assert.IsType<Game>(resp.Value);
            
            Assert.Equal(1, returnedGame.Team1);
            Assert.Equal(2, returnedGame.Team2);
            
            // default status is NotStarted
            Assert.Equal(GameStatus.NotStarted, returnedGame.Status);                        
            // 
            Assert.Null(returnedGame.CompleteDate);
            Assert.True(returnedGame.Id > 0);            
        }
       
        [DockerRequiredFact]
        public void CreateGame_EmptyTeam_ReturnsInternalServerErrorResult()
        {
             var result = controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1,                
                Goals1 = 2,
                Goals2 = 5
            });

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);            
            Assert.NotNull(jsonResult.Value);
            object? errorValue = null;
            (jsonResult.Value as Dictionary<string,object>)?.TryGetValue("error", out errorValue);
            Assert.NotNull(errorValue);

            result = controllerTests.GameController.CreateGame(new GameDTO {
                Team2 = 1,                
                Goals1 = 2,
                Goals2 = 5
            });

            // Assert
            jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, jsonResult.StatusCode);            
            Assert.NotNull(jsonResult.Value);
            errorValue = null;            
            (jsonResult.Value as Dictionary<string,object>)?.TryGetValue("error", out errorValue);
            Assert.NotNull(errorValue);
        }

        [DockerRequiredFact]
        public void CreateGame_SameTeams_ReturnsInternalServerErrorResult()
        {
             var result = controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1,              
                Team2 = 1,  
                Goals1 = 2,
                Goals2 = 5
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
        public void CreateGame_NegativeTeam_ReturnsInternalServerErrorResult()
        {
             var result = controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = -1,              
                Team2 = 1                
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