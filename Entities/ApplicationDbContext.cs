using Entities;
using Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiJwt
{
    public class ApplicationDbContext : IdentityDbContext 
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }

        public DbSet<WebAppUser> WebApp {get; set;}
        public DbSet<Todo> Todos {get; set;}

        private static string GetConnectionString()
        {
            const string databaseName = "webapijwt";
            const string databaseUser = "sa";
            // const string databasePass = "123";
            const string databasePass = "reallyStrongPwd123";
            
            return $"Server=localhost;" +
                   $"Database={databaseName};" +
                   $"User Id={databaseUser};" +
                   $"Password={databasePass};";
        }
    }
}