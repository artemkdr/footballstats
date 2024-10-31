using System.Net;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class GameController_UpdateGameTests : IDisposable
    {
        private ControllersTests controllerTests;

        public GameController_UpdateGameTests()
        {
            controllerTests = new ControllersTests();
            controllerTests.GameContext.Database.BeginTransaction();            
        }

        public void Dispose() {
            controllerTests.GameContext.Database.RollbackTransaction();              
        }

        [DockerRequiredFact]
        public async void UpdateGame_ExistingGame_ReturnsOkResult()
        {            
            // Arrange
            CreatedAtActionResult? gameResult = await controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1, Team2 = 2,
                Goals1 = 1, Goals2 = 3
            }) as CreatedAtActionResult;
            
            int id = (gameResult?.Value as Game)!.Id;

            // Act
            var result = await controllerTests.GameController.UpdateGame(id, new GameDTO() {
                Goals2 = 4
            }); 

            Game? updatedGame = await controllerTests.GameContext.Items.FindAsync(id);
            // Assert
            Assert.IsType<OkObjectResult>(result);            
            Assert.Equal(4, updatedGame?.Goals2);
        }

        [DockerRequiredFact]
        public async void UpdateGame_WithStatusCompleted_SetsCompleteDate()
        {            
            // Arrange
            CreatedAtActionResult? gameResult = await controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1, Team2 = 2,
                Goals1 = 1, Goals2 = 3
            }) as CreatedAtActionResult;
            
            int id = (gameResult?.Value as Game)!.Id;

            // Act
            var result = await controllerTests.GameController.UpdateGame(id, new GameDTO() {
                Status = GameStatus.Completed.ToString()
            }); 

            Game? updatedGame = await controllerTests.GameContext.Items.FindAsync(id);
            // Assert
            Assert.IsType<OkObjectResult>(result);            
            Assert.Equal(GameStatus.Completed, updatedGame?.Status);
            Assert.NotNull(updatedGame?.CompleteDate);
            // the complete date must be DateTime.Now, but let's give 5 sec max for db writing
            Assert.True(DateTime.UtcNow - updatedGame?.CompleteDate <= new TimeSpan(0, 0, 5));
        }

        [DockerRequiredFact]
        public async void UpdateGame_NonExistingGame_ReturnsNotFoundResult()
        {
            // Act
            var result = await controllerTests.GameController.UpdateGame(Guid.NewGuid().ToString().GetHashCode(), new GameDTO()); 

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);                    
            Assert.NotNull(objectResult.Value);                      
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);   
            Assert.NotNull((objectResult.Value as ProblemDetails)?.Detail);
        }  
    }

    
}