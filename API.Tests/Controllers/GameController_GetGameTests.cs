using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class GameController_GetGameTests : IDisposable
    {
        private ControllersTests controllerTests;
        
        public GameController_GetGameTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.GameContext.Database.BeginTransaction();
        }

        public void Dispose() {
            controllerTests.GameContext.Database.RollbackTransaction();            
        }

        [DockerRequiredFact]
        public void GetGame_ExistingGame_ReturnsOkResultWithGameData()
        {
            // Arrange
            DateTime date = DateTime.Now.AddDays(-10);
            
            var game = new Game { 
                Goals1 = 1,
                Goals2 = 5,
                Team1 = 1,
                Team2 = 2,                
                Vars = JsonDocument.Parse("{ \"key1\": \"value\", \"key2\": 2 }")
            };
            controllerTests.GameContext.Items.Add(game);
            controllerTests.GameContext.SaveChanges();

            // Act
            var result = controllerTests.GameController.GetGame(game.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGame = Assert.IsType<GameDTOFull>(okResult.Value);
            
            Assert.Equal(1, returnedGame.Goals1);
            Assert.Equal(5, returnedGame.Goals2);
            Assert.Equal(1, returnedGame.Team1?.Id);
            Assert.Equal(2, returnedGame.Team2?.Id);
            Assert.Equal(game.Vars, returnedGame.Vars); 
        }

        [DockerRequiredFact]
        public void GetGame_NonExistingGameReturnsNotFoundResult()
        {
            // Arrange
            var id = Math.Abs(Guid.NewGuid().ToString().GetHashCode());

            // Act
            var result = controllerTests.GameController.GetGame(id);

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