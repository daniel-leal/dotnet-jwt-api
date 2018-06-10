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

        private static string GetConnectionString()
        {
            const string databaseName = "webapijwt";
            const string databaseUser = "sa";
            const string databasePass = "reallyStrongPwd123";
            
            return $"Server=localhost;" +
                   $"Database={databaseName};" +
                   $"User Id={databaseUser};" +
                   $"Password={databasePass};";
        }
    }
}