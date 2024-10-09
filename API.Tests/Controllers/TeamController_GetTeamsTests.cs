using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class TeamController_GetTeamsTests : IDisposable
    {
        private ControllersTests controllerTests;

        public TeamController_GetTeamsTests()
        {
            controllerTests = new ControllersTests();
            controllerTests.TeamContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
            controllerTests.TeamContext.Database.RollbackTransaction();
        }

        [DockerRequiredFact]
        public void GetTeams_NoParameters_ReturnsOkResultWithAllTeams()
        {
            // Arrange
            controllerTests.TeamContext.Items.AddRange(
                new Team { Name = "Team 1", Status = TeamStatus.Active, Players = new[] { "Player1", "Player2" } },
                new Team { Name = "Team 2", Status = TeamStatus.Deleted, Players = new[] { "Player3", "Player4" } },
                new Team { Name = "Team 3", Status = TeamStatus.Active, Players = new[] { "Player5", "Player6" } }
            );
            controllerTests.TeamContext.SaveChanges();

            // Act - call GetTeams with no parameters
            var result = controllerTests.TeamController.GetTeams(); 

            // Assert - should return all teams
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(3, listDto.Total); // Check if the total number of teams is correct
            Assert.Equal(1, listDto.Page); // Check if the page number is correct (default should be 1)
            Assert.Equal(TeamController.LIST_LIMIT, listDto.PageSize); // Check if the page size is correct
        }

        [DockerRequiredFact]
        public void GetTeams_WithParameters_ReturnsOkResultWithFilteredTeams()
        {
            // Arrange
            controllerTests.TeamContext.Items.AddRange(
                new Team { Name = "Team Alpha", Status = TeamStatus.Active, Players = new[] { "Player1", "Player2" } },
                new Team { Name = "Team Beta", Status = TeamStatus.Deleted, Players = new[] { "Player3", "Player4" } },
                new Team { Name = "Team Gamma", Status = TeamStatus.Active, Players = new[] { "Player1", "Player5" } },
                new Team { Name = "Team Delta", Status = TeamStatus.Deleted, Players = new[] { "Player6", "Player7" } }
            );
            controllerTests.TeamContext.SaveChanges();

            // Act - filter by name
            var result = controllerTests.TeamController.GetTeams(name: "alpha");

            // Assert - returns teams with "alpha" in the name (case-insensitive)
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(1, listDto.Total);
            Assert.Equal("Team Alpha", (listDto.List?.FirstOrDefault(x => true) as TeamDTOLight)?.Name);

            // Act - filter by status
            result = controllerTests.TeamController.GetTeams(status: "Deleted");

            // Assert - returns inactive teams
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(2, listDto.Total);
            Assert.Equal("Team Beta", (listDto.List?.FirstOrDefault(x => true) as TeamDTOLight)?.Name);

            // Act - filter by players
            result = controllerTests.TeamController.GetTeams(players: "Player1");

            // Assert - returns teams with "Player1"
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(2, listDto.Total);
            Assert.True(listDto.List?.All(x => ((TeamDTOLight)x).Name == "Team Alpha" || ((TeamDTOLight)x).Name == "Team Gamma"));

            // Act - filter by multiple players
            result = controllerTests.TeamController.GetTeams(players: "Player1, Player2");

            // Assert - returns teams with both "Player1" and "Player2"
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(1, listDto.Total);
            Assert.Equal("Team Alpha", (listDto.List?.FirstOrDefault(x => true) as TeamDTOLight)?.Name);

            // Act - combine filters (name and status)
            result = controllerTests.TeamController.GetTeams(name: "a", status: "Active");

            // Assert - returns active teams with "a" in the name
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(2, listDto.Total); 
            Assert.True(listDto.List?.All(x => (((TeamDTOLight)x).Name == "Team Alpha" || ((TeamDTOLight)x).Name == "Team Gamma") && ((TeamDTOLight)x).Status == TeamStatus.Active.ToString()));
        }

        [DockerRequiredFact]
        public void GetTeams_Pagination_ReturnsOkResultWithNPages()
        {
            // Arrange
            controllerTests.TeamContext.Items.AddRange(
                new Team { Name = "Team A", Status = TeamStatus.Active, Players = new[] { "Player1", "Player2" } },
                new Team { Name = "Team B", Status = TeamStatus.Deleted, Players = new[] { "Player3", "Player4" } },
                new Team { Name = "Team C", Status = TeamStatus.Active, Players = new[] { "Player5", "Player6" } },
                new Team { Name = "Team D", Status = TeamStatus.Deleted, Players = new[] { "Player7", "Player8" } },
                new Team { Name = "Team E", Status = TeamStatus.Active, Players = new[] { "Player9", "Player10" } },
                new Team { Name = "Team F", Status = TeamStatus.Active, Players = new[] { "Player11", "Player12" } }
            );
            controllerTests.TeamContext.SaveChanges();
            
            // Act - get all teams, page 1
            var result = controllerTests.TeamController.GetTeams(page: 1, limit: 2);

            // Assert - returns 1st page
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(1, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());

            // Act - get all teams, page 2
            result = controllerTests.TeamController.GetTeams(page: 2, limit: 2);

            // Assert - returns 2nd page
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(2, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());

            // Act - get all teams, page 3
            result = controllerTests.TeamController.GetTeams(page: 3, limit: 2);

            // Assert - returns 3rd page
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);
            Assert.Equal(6, listDto.Total);
            Assert.Equal(3, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());

            // Act - get all teams, page 4 (out of range)
            result = controllerTests.TeamController.GetTeams(page: 4, limit: 2);

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