using API.Controllers;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests.Controllers
{
    public class UserController_GetUsersTests : IDisposable
    {
        private ControllersTests controllerTests;

        public UserController_GetUsersTests()
        {
            controllerTests = new ControllersTests();
            controllerTests.UserContext.Database.BeginTransaction();
        }

        public void Dispose() {
            controllerTests.UserContext.Database.RollbackTransaction();            
        }

        [DockerRequiredFact]
        public async void GetUsers_NoParameters_ReturnsOkResultWithAllUsers()
        {   
            // Arrange
            controllerTests.UserContext.Items.AddRange(
                new User { Username = Guid.NewGuid().ToString(), Status = UserStatus.Active },
                new User { Username = Guid.NewGuid().ToString(), Status = UserStatus.Deleted },
                new User { Username = Guid.NewGuid().ToString(), Status = UserStatus.Active }
            );
            await controllerTests.UserContext.SaveChangesAsync();
            
            // Act
            var result = await controllerTests.UserController.GetUsers(); // No parameters

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(3, listDto.Total);
            Assert.Equal(1, listDto.Page);
            Assert.Equal(UserService.LIST_LIMIT, listDto.PageSize);             
        }


        [DockerRequiredFact]
        public async void GetUsers_WithParameters_ReturnsOkResultWithFilteredUsers()
        {  
            // Arrange
            controllerTests.UserContext.Items.AddRange(
                new User { Username = "user1", Status = UserStatus.Active },
                new User { Username = "user2", Status = UserStatus.Deleted },
                new User { Username = "user3", Status = UserStatus.Active },
                new User { Username = "user4", Status = UserStatus.Active },
                new User { Username = "test5", Status = UserStatus.Active }
            );
            await controllerTests.UserContext.SaveChangesAsync();
            
            // Act - filter by Status=Deleted
            var result = await controllerTests.UserController.GetUsers(status: "Deleted"); // filter by status

            // Assert - returns 1 user
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(1, listDto.Total);            
            Assert.Equal(1, listDto.List?.Count());
            Assert.Equal(UserStatus.Deleted.ToString(), (listDto.List?.FirstOrDefault(x => true) as UserDTO)?.Status);

            // Act - filter by exact Username
            result = await controllerTests.UserController.GetUsers("user2"); // filter by username

            // Assert - returns 1 user
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(1, listDto.Total);            
            Assert.Equal(1, listDto.List?.Count());    
            Assert.Equal("user2", (listDto.List?.FirstOrDefault(x => true) as UserDTO)?.Username);

            // Act - search by username
            result = await controllerTests.UserController.GetUsers("ser"); // search by part of username

            // Assert - returns 4 users with 'ser' in username
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(4, listDto.Total);            
            Assert.Equal(4, listDto.List?.Count());
            Assert.True(listDto.List?.All(x => (x as UserDTO)?.Username?.Contains("ser") == true));

            // Act - search by username case insensitive
            result = await controllerTests.UserController.GetUsers("sEr"); // search by part of username (case insensitive)

            // Assert - returns 4 users with 'ser' in username
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(4, listDto.Total);            
            Assert.Equal(4, listDto.List?.Count());
            Assert.True(listDto.List?.All(x => (x as UserDTO)?.Username?.Contains("ser") == true));

            // Act - returns empty list if nothing found
            result = await controllerTests.UserController.GetUsers("yyyyyyyy"); // search by part of username

            // Assert - returns 0 users with 'yyyyyyyy' in username
            okResult = Assert.IsType<OkObjectResult>(result);
            listDto = Assert.IsType<ListDTO>(okResult.Value);

            Assert.Equal(0, listDto.Total);            
            Assert.Equal(0, listDto.List?.Count());    
        }  

        [DockerRequiredFact]
        public async void GetUsers_Pagination_ReturnsOkResultNUsersByPage()
        {   
            // Arrange
            controllerTests.UserContext.Items.AddRange(
                new User { Username = "user1", Status = UserStatus.Active },
                new User { Username = "user2", Status = UserStatus.Deleted },
                new User { Username = "user3", Status = UserStatus.Active },
                new User { Username = "user4", Status = UserStatus.Active },
                new User { Username = "user5", Status = UserStatus.Active },
                new User { Username = "user6", Status = UserStatus.Active }
            );
            await controllerTests.UserContext.SaveChangesAsync();

            // Act - get all users, page 1
            var result = await controllerTests.UserController.GetUsers(page: 1, limit: 2); 

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
            result = await controllerTests.UserController.GetUsers(page: 2, limit: 2); 

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
            result = await controllerTests.UserController.GetUsers(page: 3, limit: 2); 

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
            result = await controllerTests.UserController.GetUsers(page: 4, limit: 2); 

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
            result = await controllerTests.UserController.GetUsers(page: 2, limit: 4); 

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
        }  
    }

    
}