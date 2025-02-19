using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Data
{
    public class TangyuanDbContext : DbContext
    {
        public TangyuanDbContext(DbContextOptions<TangyuanDbContext> options) : base(options)
        {
            Database.EnsureCreated();

            if (!Database.IsInMemory())
            {
                Database.Migrate();
            }
        }

        public DbSet<Models.PostMetadata> PostMetadata { get; set; } = null;
    }
}
