using API.Controllers;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace API.Tests.Controllers
{
    public class ControllersTests
    {
        private static DbContextOptions<BaseDBContext<User>> optionsUser = 
            new DbContextOptionsBuilder<BaseDBContext<User>>()
                .UseNpgsql(TestContainers.GetConnectionString()).Options;

        private static DbContextOptions<BaseDBContext<Team>> optionsTeam = 
            new DbContextOptionsBuilder<BaseDBContext<Team>>()
                .UseNpgsql(TestContainers.GetConnectionString()).Options;

        private static DbContextOptions<BaseDBContext<Game>> optionsGame = 
            new DbContextOptionsBuilder<BaseDBContext<Game>>()
                .UseNpgsql(TestContainers.GetConnectionString()).Options;

        public BaseDBContext<User> UserContext { get; }
        
        public BaseDBContext<Team> TeamContext { get; }
        
        public BaseDBContext<Game> GameContext { get; }

        public IUserService UserService { get; }
        
        private readonly Mock<IConfiguration> _configurationMock;
        
        public UserController UserController { get; }

        public TeamController TeamController { get; }

        public GameController GameController { get; }

        public StatsController StatsController { get; }
        
        public ControllersTests()
        {  
            UserContext = new BaseDBContext<User>(optionsUser); 
            TeamContext = new BaseDBContext<Team>(optionsTeam); 
            GameContext = new BaseDBContext<Game>(optionsGame); 
            UserService = new UserService(UserContext, TeamContext);

            _configurationMock = new Mock<IConfiguration>();
            
            UserController = new UserController(_configurationMock.Object, UserContext, TeamContext, GameContext, UserService);
            TeamController = new TeamController(_configurationMock.Object, UserContext, TeamContext, GameContext);
            GameController = new GameController(_configurationMock.Object, UserContext, TeamContext, GameContext);
            StatsController = new StatsController(_configurationMock.Object, UserContext, TeamContext, GameContext);
        }                
    }
}