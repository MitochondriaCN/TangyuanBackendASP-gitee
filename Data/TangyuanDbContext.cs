using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Data
{
    public class TangyuanDbContext : DbContext
    {
        public TangyuanDbContext(DbContextOptions<TangyuanDbContext> options) : base(options) { }

        public DbSet<Models.PostMetadata> PostMetadata { get; set; } = null;
    }
}
