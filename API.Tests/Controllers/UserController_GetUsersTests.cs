using System.Net;
using System.Text.Json;
using API.Controllers;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace API.Tests.Controllers
{
    public class UserController_GetUsersTests
    {
        private readonly BaseDBContext<User> _userContext;
        private readonly BaseDBContext<Team> _teamContext;
        private readonly BaseDBContext<Game> _gameContext;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserController _controller;
        
        public UserController_GetUsersTests()
        {  
            var optionsUser = new DbContextOptionsBuilder<BaseDBContext<User>>().UseNpgsql(TestContainers.GetConnectionString()).Options;
            var optionsTeam = new DbContextOptionsBuilder<BaseDBContext<Team>>().UseNpgsql(TestContainers.GetConnectionString()).Options;
            var optionsGame = new DbContextOptionsBuilder<BaseDBContext<Game>>().UseNpgsql(TestContainers.GetConnectionString()).Options;

            _userContext = new BaseDBContext<User>(optionsUser); 
            _teamContext = new BaseDBContext<Team>(optionsTeam); 
            _gameContext = new BaseDBContext<Game>(optionsGame); 
            _configurationMock = new Mock<IConfiguration>();
            _controller = new UserController(_configurationMock.Object, _userContext, _teamContext, _gameContext);
        }

        
        [DockerRequiredFact]
        public void GetUsers_NoParameters_ReturnsOkResultWithAllUsers()
        {     
            _userContext.Database.BeginTransaction();

            EFUtils.ClearTable(_userContext, EFUtils.GetTableName(typeof(User)));
            EFUtils.ClearTable(_teamContext, EFUtils.GetTableName(typeof(Team)));

            // Arrange
            _userContext.Items.AddRange(
                new User { Username = "user1", Status = UserStatus.Active },
                new User { Username = "user2", Status = UserStatus.Deleted },
                new User { Username = "user3", Status = UserStatus.Active }
            );
            _userContext.SaveChanges();
            
            // Act
            var result = _controller.GetUsers(); // No parameters

            _userContext.Database.RollbackTransaction();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(3, listDto.Total);
            Assert.Equal(1, listDto.Page);
            Assert.Equal(UserController.LIST_LIMIT, listDto.PageSize);             
        }


        [DockerRequiredFact]
        public void GetUsers_WithParameters_ReturnsOkResultWithFilteredUsers()
        {    
            _userContext.Database.BeginTransaction();

            EFUtils.ClearTable(_userContext, EFUtils.GetTableName(typeof(User)));
            EFUtils.ClearTable(_teamContext, EFUtils.GetTableName(typeof(Team)));

            // Arrange
            _userContext.Items.AddRange(
                new User { Username = "user1", Status = UserStatus.Active },
                new User { Username = "user2", Status = UserStatus.Deleted },
                new User { Username = "user3", Status = UserStatus.Active },
                new User { Username = "user4", Status = UserStatus.Active },
                new User { Username = "test5", Status = UserStatus.Active }
            );
            _userContext.SaveChanges();
            
            // Act - filter by Status=Deleted
            var result = _controller.GetUsers(null, "Deleted"); // filter by status

            // Assert - returns 1 user
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(1, listDto.Total);            
            Assert.Equal(1, listDto.List?.Count());
            Assert.Equal(UserStatus.Deleted.ToString(), (listDto.List?.FirstOrDefault(x => true) as UserDTO)?.Status);

            // Act - filter by exact Username
            result = _controller.GetUsers("user2"); // filter by username

            // Assert - returns 1 user
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(1, listDto.Total);            
            Assert.Equal(1, listDto.List?.Count());    
            Assert.Equal("user2", (listDto.List?.FirstOrDefault(x => true) as UserDTO)?.Username);

            // Act - search by username
            result = _controller.GetUsers("ser"); // search by part of username

            // Assert - returns 4 users with 'ser' in username
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(4, listDto.Total);            
            Assert.Equal(4, listDto.List?.Count());
            Assert.True(listDto.List?.All(x => (x as UserDTO)?.Username?.Contains("ser") == true));

            // Act - search by username case insensitive
            result = _controller.GetUsers("sEr"); // search by part of username (case insensitive)

            // Assert - returns 4 users with 'ser' in username
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(4, listDto.Total);            
            Assert.Equal(4, listDto.List?.Count());
            Assert.True(listDto.List?.All(x => (x as UserDTO)?.Username?.Contains("ser") == true));

            // Act - returns empty list if nothing found
            result = _controller.GetUsers("yyyyyyyy"); // search by part of username

            _userContext.Database.RollbackTransaction();

            // Assert - returns 0 users with 'yyyyyyyy' in username
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(0, listDto.Total);            
            Assert.Equal(0, listDto.List?.Count());    
        }  

        [DockerRequiredFact]
        public void GetUsers_Pagination_ReturnsOkResultNUsersByPage()
        {         
            _userContext.Database.BeginTransaction();

            EFUtils.ClearTable(_userContext, EFUtils.GetTableName(typeof(User)));
            EFUtils.ClearTable(_teamContext, EFUtils.GetTableName(typeof(Team)));

            // Arrange
            _userContext.Items.AddRange(
                new User { Username = "user1", Status = UserStatus.Active },
                new User { Username = "user2", Status = UserStatus.Deleted },
                new User { Username = "user3", Status = UserStatus.Active },
                new User { Username = "user4", Status = UserStatus.Active },
                new User { Username = "user5", Status = UserStatus.Active },
                new User { Username = "user6", Status = UserStatus.Active }
            );
            _userContext.SaveChanges();

            int initialLImit = UserController.LIST_LIMIT;

            UserController.LIST_LIMIT = 2;
            
            // Act - get all users, page 1
            var result = _controller.GetUsers(null, null, 1); 

            // Assert - returns 1st page
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(6, listDto.Total);            
            Assert.Equal(1, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());
            Assert.Equal("user1", (listDto.List?[0] as UserDTO)?.Username);
            Assert.Equal("user2", (listDto.List?[1] as UserDTO)?.Username);

            // Act - get all users, page 2
            result = _controller.GetUsers(null, null, 2); 

            // Assert - returns 2nd page
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(6, listDto.Total);            
            Assert.Equal(2, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());
            Assert.Equal("user3", (listDto.List?[0] as UserDTO)?.Username);
            Assert.Equal("user4", (listDto.List?[1] as UserDTO)?.Username);

            // Act - get all users, page 3
            result = _controller.GetUsers(null, null, 3); 

            // Assert - returns the last page
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(6, listDto.Total);            
            Assert.Equal(3, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());
            Assert.Equal("user5", (listDto.List?[0] as UserDTO)?.Username);
            Assert.Equal("user6", (listDto.List?[1] as UserDTO)?.Username);

            // Act - get all users, page 4
            result = _controller.GetUsers(null, null, 4); 

            // Assert - returns last page if the page index exceeds 
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(6, listDto.Total);            
            Assert.Equal(3, listDto.Page);
            Assert.Equal(2, listDto.PageSize);
            Assert.Equal(3, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());
            Assert.Equal("user5", (listDto.List?[0] as UserDTO)?.Username);
            Assert.Equal("user6", (listDto.List?[1] as UserDTO)?.Username);

            // Act - get all users, page 2 with 4 users per page
            UserController.LIST_LIMIT = 4;
            result = _controller.GetUsers(null, null, 2); 

            _userContext.Database.RollbackTransaction();
            
            // Assert - returns last page if the page index exceeds 
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(6, listDto.Total);            
            Assert.Equal(2, listDto.Page);
            Assert.Equal(4, listDto.PageSize);
            Assert.Equal(2, listDto.TotalPages);
            Assert.Equal(2, listDto.List?.Count());
            Assert.Equal("user5", (listDto.List?[0] as UserDTO)?.Username);
            Assert.Equal("user6", (listDto.List?[1] as UserDTO)?.Username);


            UserController.LIST_LIMIT = initialLImit;
        }  
    }

    
}