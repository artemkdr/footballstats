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
        public void UpdateGame_ExistingGame_ReturnsOkResult()
        {            
            // Arrange
            CreatedAtActionResult? gameResult = controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1, Team2 = 2,
                Goals1 = 1, Goals2 = 3
            }) as CreatedAtActionResult;
            
            int id = (gameResult?.Value as Game)!.Id;

            // Act
            var result = controllerTests.GameController.UpdateGame(id, new GameDTO() {
                Goals2 = 4
            }); 

            Game? updatedGame = controllerTests.GameContext.Items.Find(id);
            // Assert
            Assert.IsType<OkObjectResult>(result);            
            Assert.Equal(4, updatedGame?.Goals2);
        }

        [DockerRequiredFact]
        public void UpdateGame_WithStatusCompleted_SetsCompleteDate()
        {            
            // Arrange
            CreatedAtActionResult? gameResult = controllerTests.GameController.CreateGame(new GameDTO {
                Team1 = 1, Team2 = 2,
                Goals1 = 1, Goals2 = 3
            }) as CreatedAtActionResult;
            
            int id = (gameResult?.Value as Game)!.Id;

            // Act
            var result = controllerTests.GameController.UpdateGame(id, new GameDTO() {
                Status = GameStatus.Completed.ToString()
            }); 

            Game? updatedGame = controllerTests.GameContext.Items.Find(id);
            // Assert
            Assert.IsType<OkObjectResult>(result);            
            Assert.Equal(GameStatus.Completed, updatedGame?.Status);
            Assert.NotNull(updatedGame?.CompleteDate);
            // the complete date must be DateTime.Now, but let's give 5 sec max for db writing
            Assert.True(DateTime.UtcNow - updatedGame?.CompleteDate <= new TimeSpan(0, 0, 5));
        }

        [DockerRequiredFact]
        public void UpdateGame_NonExistingGame_ReturnsNotFoundResult()
        {
            // Act
            var result = controllerTests.GameController.UpdateGame(Guid.NewGuid().ToString().GetHashCode(), new GameDTO()); 

            // Assert
            var typedResult = Assert.IsType<NotFoundObjectResult>(result);                    
            Assert.NotNull(typedResult.Value);                        
            Assert.NotNull((typedResult.Value as ErrorDTO)?.Error ?? (typedResult.Value as ErrorDTO)?.Detail);
        }  
    }

    
}