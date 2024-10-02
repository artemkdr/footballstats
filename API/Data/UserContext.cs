using API.Models; // Assuming you create a Models folder for your User class
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } 
    }
}