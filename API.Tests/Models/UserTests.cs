using System.Text.Json;
using API.Models;

namespace API.Tests.Models {
    public class UsersTest {
        
        [Fact]
        public void User_SerializeWithoutPassword() {
            var json = JsonSerializer.Serialize(new User {
                Username = "test",
                Password = "test",
                Status = UserStatus.Active,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now                
            });

            var user = JsonSerializer.Deserialize<User>(json);
            Assert.NotNull(user);
            Assert.Null(user!.Password);
        }
    }
}