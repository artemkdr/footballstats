using API.Controllers;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class GameController_GetGamesTests : IDisposable
    {
        private ControllersTests controllerTests;

        public GameController_GetGamesTests()
        {
            controllerTests = new ControllersTests();
            controllerTests.GameContext.Database.BeginTransaction();
            controllerTests.TeamContext.Database.BeginTransaction(); 
        }

        public void Dispose()
        {
            controllerTests.GameContext.Database.RollbackTransaction();
            controllerTests.TeamContext.Database.RollbackTransaction();
        }

        [DockerRequiredFact]
        public async void GetGames_NoParameters_ReturnsOkResultWithAllGames()
        {
            // Arrange
            var team1 = new Team { Name = "Team 1", Players = new[] { "Player1", "Player2" } };
            var team2 = new Team { Name = "Team 2", Players = new[] { "Player3", "Player4" } };
            controllerTests.TeamContext.Items.AddRange(team1, team2);
            await controllerTests.TeamContext.SaveChangesAsync();

            controllerTests.GameContext.Items.AddRange(
                new Game { Team1 = team1.Id, Team2 = team2.Id, Status = GameStatus.Playing, ModifyDate = DateTime.Now.AddDays(-2).ToUniversalTime() },
                new Game { Team1 = team2.Id, Team2 = team1.Id, Status = GameStatus.Completed, CompleteDate = DateTime.Now.AddDays(-3).ToUniversalTime(), ModifyDate = DateTime.Now.AddDays(-4).ToUniversalTime() },
                new Game { Team1 = team1.Id, Team2 = team2.Id, Status = GameStatus.NotStarted, ModifyDate = DateTime.Now.AddDays(-5).ToUniversalTime() }
            );
            await controllerTests.GameContext.SaveChangesAsync();

            // Act
            var result = await controllerTests.GameController.GetGames(); // No parameters

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(3, listDto.Total);
            Assert.Equal(1, listDto.Page);
            Assert.Equal(UserService.LIST_LIMIT, listDto.PageSize);
        }

        [DockerRequiredFact]
        public async void GetGames_WithParameters_ReturnsOkResultWithFilteredGames()
        {
            // Arrange
            var team1 = new Team { Name = "Team A", Players = new[] { "PlayerA1", "PlayerA2" } };
            var team2 = new Team { Name = "Team B", Players = new[] { "PlayerB1", "PlayerB2" } };
            var team3 = new Team { Name = "Team C", Players = new[] { "PlayerC1", "PlayerC2" } };
            controllerTests.TeamContext.Items.AddRange(team1, team2, team3);
            await controllerTests.TeamContext.SaveChangesAsync();

            controllerTests.GameContext.Items.AddRange(
                new Game { Team1 = team1.Id, Team2 = team2.Id, Status = GameStatus.Playing, ModifyDate = DateTime.Now.AddDays(-1).ToUniversalTime() },
                new Game { Team1 = team2.Id, Team2 = team1.Id, Status = GameStatus.Completed, CompleteDate = DateTime.Now.AddDays(-2).ToUniversalTime(), ModifyDate = DateTime.Now.AddDays(-2).ToUniversalTime() },
                new Game { Team1 = team1.Id, Team2 = team3.Id, Status = GameStatus.NotStarted, ModifyDate = DateTime.Now.AddDays(-3).ToUniversalTime() },
                new Game { Team1 = team3.Id, Team2 = team2.Id, Status = GameStatus.Completed, CompleteDate = DateTime.Now.AddDays(-4).ToUniversalTime(), ModifyDate = DateTime.Now.AddDays(-4).ToUniversalTime() }
            );
            await controllerTests.GameContext.SaveChangesAsync();

            // Act - filter by Status=Completed
            var result = await controllerTests.GameController.GetGames(status: "Completed");

            // Assert - returns 2 completed games
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(2, listDto.Total);
            Assert.True(listDto.List?.All(x => (x as GameDTO)?.Status == GameStatus.Completed.ToString()));

            // Act - filter by team1
            result = await controllerTests.GameController.GetGames(team1: team1.Id);

            // Assert - returns 3 games with team1
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(3, listDto.Total);
            Assert.True(listDto.List?.All(x => (x as GameDTO)?.Team1Detail?.Id == team1.Id || (x as GameDTO)?.Team2Detail?.Id == team1.Id));

            // Act - filter by team1 and team2
            result = await controllerTests.GameController.GetGames(team1: team1.Id, team2: team2.Id);

            // Assert - returns 2 games between team1 and team2
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(2, listDto.Total);
            
            // Act - filter by players
            result = await controllerTests.GameController.GetGames(players: "PlayerA1, PlayerB1");

            // Assert - returns 4 games with PlayerA1 and PlayerB1
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(4, listDto.Total); 

            // Act - filter by fromDate
            var fromDate = DateTime.Now.AddDays(-2).ToUniversalTime();
            result = await controllerTests.GameController.GetGames(fromDate: fromDate);

            // Assert - returns 1 game completed after fromDate
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(1, listDto.Total);
            Assert.True(listDto.List?.All(x => (x as GameDTO)?.CompleteDate?.Date >= fromDate.Date));

            // Act - filter by toDate
            var toDate = DateTime.Now.AddDays(-3).ToUniversalTime();
            result = await controllerTests.GameController.GetGames(toDate: toDate);

            // Assert - returns 1 game completed before toDate
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(1, listDto.Total);
            Assert.True(listDto.List?.All(x => (x as GameDTO)?.CompleteDate?.Date <= toDate.Date));
        }

        [DockerRequiredFact]
        public async void GetGames_Pagination_ReturnsOkResultWithNPages()
        {
            // Arrange
            var team1 = new Team { Name = "Team 1", Players = new[] { "Player1", "Player2" } };
            var team2 = new Team { Name = "Team 2", Players = new[] { "Player3", "Player4" } };
            controllerTests.TeamContext.Items.AddRange(team1, team2);
            await controllerTests.TeamContext.SaveChangesAsync();

            controllerTests.GameContext.Items.AddRange(
                new Game { Team1 = team1.Id, Team2 = team2.Id, Status = GameStatus.Playing, ModifyDate = DateTime.Now.AddDays(-1).ToUniversalTime() },
                new Game { Team1 = team2.Id, Team2 = team1.Id, Status = GameStatus.Completed, CompleteDate = DateTime.Now.AddDays(-2).ToUniversalTime(), ModifyDate = DateTime.Now.AddDays(-2).ToUniversalTime() },
                new Game { Team1 = team1.Id, Team2 = team2.Id, Status = GameStatus.NotStarted, ModifyDate = DateTime.Now.AddDays(-3).ToUniversalTime() },
                new Game { Team1 = team2.Id, Team2 = team1.Id, Status = GameStatus.Completed, CompleteDate = DateTime.Now.AddDays(-4).ToUniversalTime(), ModifyDate = DateTime.Now.AddDays(-4).ToUniversalTime() },
                new Game { Team1 = team1.Id, Team2 = team2.Id, Status = GameStatus.Playing, ModifyDate = DateTime.Now.AddDays(-5).ToUniversalTime() },
                new Game { Team1 = team2.Id, Team2 = team1.Id, Status = GameStatus.NotStarted,ModifyDate = DateTime.Now.AddDays(-6).ToUniversalTime() } 
            );
            await controllerTests.GameContext.SaveChangesAsync();

            // Act - get all games, page 1
            var result = await controllerTests.GameController.GetGames(page: 1, limit: 2);

            // Assert - returns 1st page
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(1, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count()); 

            // Act - get all games, page 2
            result = await controllerTests.GameController.GetGames(page: 2, limit: 2);

            // Assert - returns 2nd page
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(2, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());

            // Act - get all games, page 3
            result = await controllerTests.GameController.GetGames(page: 3, limit: 2);

            // Assert - returns 3rd page
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(3, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());

            // Act - get all games, page 4 (out of range)
            result = await controllerTests.GameController.GetGames(page: 4, limit: 2);

            // Assert - returns last page (3)
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(3, listDto.Page); // Page should be adjusted to the last page
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());
        }
    }
}


        