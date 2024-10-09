using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class StatsControllerTests : IDisposable
    {
        private ControllersTests controllerTests;

        public StatsControllerTests()
        {
            controllerTests = new ControllersTests();            
            controllerTests.GameContext.Database.BeginTransaction();            
            controllerTests.TeamContext.Database.BeginTransaction();        
        }

        public void Dispose() {
            controllerTests.GameContext.Database.RollbackTransaction();                        
            controllerTests.TeamContext.Database.RollbackTransaction();                        
        }
        

        [Fact]
        public void GetStats_NoTeam_ReturnsAllStats()
        {
            // Arrange
            var teams = new List<Team>
            {
                new Team { Name = "Team A", Players = new string[] { "Player 1", "Player 2"} },
                new Team { Name = "Team B", Players = new string[] { "Player 3" } },
                new Team { Name = "Team C", Players = new string[] { "Player 4" } },                
            };            
            controllerTests.TeamContext.Items.AddRange(teams);
            controllerTests.TeamContext.SaveChanges();

            var games = new List<Game>
            {
                new Game { Team1 = 1, Team2 = 2, Goals1 = 2, Goals2 = 1, Status = GameStatus.Completed }, // 1 won 2:1, 2 lost 1:2
                new Game { Team1 = 1, Team2 = 2, Goals1 = 1, Goals2 = 1, Status = GameStatus.Completed }, // draw between 1 and 2, 1:1
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 1, Status = GameStatus.Completed }, // draw between 1 and 3, 1:1
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 2, Status = GameStatus.Playing }, // not completed
                new Game { Team1 = 1, Team2 = 3, Goals1 = 6, Goals2 = 1, Status = GameStatus.Cancelled }, // not completed
                new Game { Team1 = 2, Team2 = 1, Goals1 = 10, Goals2 = 3, Status = GameStatus.Completed }, // 2 won 10:3, 1 lost 3:10
                new Game { Team1 = 2, Team2 = 3, Goals1 = 5, Goals2 = 3, Status = GameStatus.Completed }, // 2 won 5:3, 3 lost 3:5
                new Game { Team1 = 3, Team2 = 1, Goals1 = 3, Goals2 = 3, Status = GameStatus.Completed }, // draw between 3 and 1, 3:3
                new Game { Team1 = 3, Team2 = 2, Goals1 = 10, Goals2 = 0, Status = GameStatus.Completed }, // 3 won 10:0, 2 lost 0:10
            };
            // count only completed games
            // team 2 (Team B) has 2 wins, 2 loss, 1 draw, 5 total, ratio 0.4, GA = 19, GF = 17
            // team 3 (Team C) has 1 win, 1 loss, 2 draws, 4 total, ratio 0.25, GA = 9, GF = 17
            // team 1 (Team A) has 1 wins, 1 loss, 3 draws, 5 total, ratio 0.2, GA = 10; GF = 16
            

            controllerTests.GameContext.Items.AddRange(games);
            controllerTests.GameContext.SaveChanges();
            
            // Act
            var result = controllerTests.StatsController.GetStats() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var stats = Assert.IsType<List<StatsDTO>>(result!.Value);
            Assert.Equal(3, stats.Count);

            // stats have names
            Assert.True(stats.All(x => x.Name != null));
            
            // check team 2 stats             
            Assert.Equal(2, stats[0].Id);
            Assert.Equal(2, stats[0].Wins);
            Assert.Equal(2, stats[0].Losses);
            Assert.Equal(5, stats[0].Games);
            Assert.Equal(17, stats[0].GF);
            Assert.Equal(19, stats[0].GA);
            
            // check team 1 stats 
            Assert.Equal(3, stats[1].Id);            
            Assert.Equal(1, stats[1].Wins);
            Assert.Equal(1, stats[1].Losses);
            Assert.Equal(4, stats[1].Games);
            Assert.Equal(17, stats[1].GF);
            Assert.Equal(9, stats[1].GA);            

            // check team 3 stats 
            Assert.Equal(1, stats[2].Id);                    
            Assert.Equal(1, stats[2].Wins);
            Assert.Equal(1, stats[2].Losses);
            Assert.Equal(5, stats[2].Games);
            Assert.Equal(10, stats[2].GF);
            Assert.Equal(16, stats[2].GA);            
        }

        [Fact]
        public void GetStats_NoTeam_ReturnsAllStats_CheckSort()
        {
            // Arrange
            var teams = new List<Team>
            {
                new Team { Name = "Team A", Players = new string[] { "Player 1", "Player 2"} },
                new Team { Name = "Team B", Players = new string[] { "Player 3" } }                
            };            
            controllerTests.TeamContext.Items.AddRange(teams);
            controllerTests.TeamContext.SaveChanges();

            var games = new List<Game>
            {
                new Game { Team1 = 1, Team2 = 2, Goals1 = 5, Goals2 = 1, Status = GameStatus.Completed },
                new Game { Team1 = 2, Team2 = 1, Goals1 = 3, Goals2 = 1, Status = GameStatus.Completed },
                
            };
            // count only completed games
            // team 1 (Team A) has 1 win, 1 loss, 2 total, ratio 0.5, GA = 4, GF = 6 - goals diff +2
            // team 2 (Team B) has 1 win, 1 loss, 2 total, ratio 0.5, GA = 6, GF = 4 - goals diff -2 
                        
            controllerTests.GameContext.Items.AddRange(games);
            controllerTests.GameContext.SaveChanges();
            
            // Act
            var result = controllerTests.StatsController.GetStats() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var stats = Assert.IsType<List<StatsDTO>>(result!.Value);
            Assert.Equal(2, stats.Count);
            
            // check team 1 stats             
            Assert.Equal(1, stats[0].Id);
            Assert.Equal(1, stats[0].Wins);
            Assert.Equal(1, stats[0].Losses);
            Assert.Equal(2, stats[0].Games);
            Assert.Equal(6, stats[0].GF);
            Assert.Equal(4, stats[0].GA);
            
            // check team 2 stats 
            Assert.Equal(2, stats[1].Id);            
            Assert.Equal(1, stats[1].Wins);
            Assert.Equal(1, stats[1].Losses);
            Assert.Equal(2, stats[1].Games);
            Assert.Equal(4, stats[1].GF);
            Assert.Equal(6, stats[1].GA);
        }

        [Fact]
        public void GetStats_WithTeam_ReturnsStatsForTeam()
        {
            // Arrange
            var teams = new List<Team>
            {
                new Team { Name = "Team A", Players = new string[] { "Player 1", "Player 2"} },
                new Team { Name = "Team B", Players = new string[] { "Player 3" } },
                new Team { Name = "Team C", Players = new string[] { "Player 4" } },                
            };            
            controllerTests.TeamContext.Items.AddRange(teams);
            controllerTests.TeamContext.SaveChanges();

            var games = new List<Game>
            {
                new Game { Team1 = 1, Team2 = 2, Goals1 = 2, Goals2 = 1, Status = GameStatus.Completed }, // 1 won 2:1, 2 lost 1:2
                new Game { Team1 = 1, Team2 = 2, Goals1 = 1, Goals2 = 1, Status = GameStatus.Completed }, // draw between 1 and 2, 1:1
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 1, Status = GameStatus.Completed }, // draw between 1 and 3, 1:1
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 2, Status = GameStatus.Playing }, // not completed
                new Game { Team1 = 1, Team2 = 3, Goals1 = 6, Goals2 = 1, Status = GameStatus.Cancelled }, // not completed
                new Game { Team1 = 2, Team2 = 1, Goals1 = 10, Goals2 = 3, Status = GameStatus.Completed }, // 2 won 10:3, 1 lost 3:10
                new Game { Team1 = 2, Team2 = 3, Goals1 = 5, Goals2 = 3, Status = GameStatus.Completed }, // 2 won 5:3, 3 lost 3:5
                new Game { Team1 = 3, Team2 = 1, Goals1 = 3, Goals2 = 3, Status = GameStatus.Completed }, // draw between 3 and 1, 3:3
                new Game { Team1 = 3, Team2 = 2, Goals1 = 10, Goals2 = 0, Status = GameStatus.Completed }, // 3 won 10:0, 2 lost 0:10
            };
            // count only completed games
            // team 2 (Team B) has 2 wins, 2 loss, 1 draw, 5 total, ratio 0.4, GA = 19, GF = 17
            
            controllerTests.GameContext.Items.AddRange(games);
            controllerTests.GameContext.SaveChanges();
            
            // Act
            var result = controllerTests.StatsController.GetStats(2) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var stats = Assert.IsType<List<StatsDTO>>(result!.Value);
            Assert.True(stats.Count == 1);
            // check team 2 stats 
            Assert.Equal(2, stats[0].Id);
            Assert.Equal(2, stats[0].Wins);
            Assert.Equal(2, stats[0].Losses);
            Assert.Equal(5, stats[0].Games);
            Assert.Equal(17, stats[0].GF);
            Assert.Equal(19, stats[0].GA);
            Assert.NotNull(stats[0].Name);
        }
        

        [Fact]
        public void GetStatsRivals_ReturnsRivalsStats()
        {
            // Arrange
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team A", Players = new string[] { "Player 1", "Player 2"} },
                new Team { Id = 2, Name = "Team B", Players = new string[] { "Player 3" } },
                new Team { Id = 3, Name = "Team C", Players = new string[] { "Player 4" } }
            };            
            controllerTests.TeamContext.Items.AddRange(teams);
            controllerTests.TeamContext.SaveChanges();
            
            var games = new List<Game>
            {
                new Game { Team1 = 1, Team2 = 2, Goals1 = 2, Goals2 = 1, Status = GameStatus.Completed },
                new Game { Team1 = 1, Team2 = 2, Goals1 = 1, Goals2 = 1, Status = GameStatus.Completed },
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 1, Status = GameStatus.Completed },
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 1, Status = GameStatus.Playing },
                new Game { Team1 = 1, Team2 = 3, Goals1 = 1, Goals2 = 1, Status = GameStatus.Cancelled },
                new Game { Team1 = 2, Team2 = 1, Goals1 = 10, Goals2 = 3, Status = GameStatus.Completed },
                new Game { Team1 = 2, Team2 = 3, Goals1 = 5, Goals2 = 3, Status = GameStatus.Completed },
                new Game { Team1 = 3, Team2 = 1, Goals1 = 3, Goals2 = 3, Status = GameStatus.Completed },
                new Game { Team1 = 3, Team2 = 2, Goals1 = 10, Goals2 = 0, Status = GameStatus.Completed },                
            };
            controllerTests.GameContext.Items.AddRange(games);
            controllerTests.GameContext.SaveChanges();
            
            // Act
            var result = controllerTests.StatsController.GetStatsRivals(1, 2) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var stats = Assert.IsType<StatsRivalsDTO>(result?.Value);
            Assert.Equal(1, stats.Wins1); 
            Assert.Equal(1, stats.Wins2);
            Assert.Equal(1, stats.Draws);
        }

        [Fact]
        public void GetStats_NoGames_ReturnsEmptyStats()
        {
            // Act
            var result = controllerTests.StatsController.GetStats() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var stats = Assert.IsType<List<StatsDTO>>(result!.Value);
            Assert.Empty(stats); 
            
        }
    }
}