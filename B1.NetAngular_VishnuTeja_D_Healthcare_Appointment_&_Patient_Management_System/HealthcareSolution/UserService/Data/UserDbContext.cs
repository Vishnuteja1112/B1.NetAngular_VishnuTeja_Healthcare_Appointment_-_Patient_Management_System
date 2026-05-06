using Microsoft.EntityFrameworkCore;
using UserService.Models; 


namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "VishnuTeja",
                    Password = "$2a$11$qKok1.5ky1uIWqpC5y8n3ubyU9CNIEqEbJgsfhu4uuYfmngvDzZcy",
                    Role = "Admin"
                }
            );
        }
    }
}