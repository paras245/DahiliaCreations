using Microsoft.EntityFrameworkCore;

namespace DahiliaCreations.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Fish> Fish { get; set; }
    }

}