using API.Models; // Assuming you create a Models folder for your User class
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BaseDBContext<T> : DbContext where T : class
    {
        public BaseDBContext(DbContextOptions<BaseDBContext<T>> options) : base(options)
        {
        }

        public DbSet<T> Items { get; set; } 
    }
}