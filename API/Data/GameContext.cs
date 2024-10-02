using API.Models; // Assuming you create a Models folder for your User class
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } 
    }
}