using API.Models; // Assuming you create a Models folder for your User class
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class TeamContext : DbContext
    {
        public TeamContext(DbContextOptions<TeamContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; } 
    }
}