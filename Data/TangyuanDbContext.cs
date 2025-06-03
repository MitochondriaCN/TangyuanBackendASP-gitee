using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Data
{
    public class TangyuanDbContext : DbContext
    {
        public TangyuanDbContext(DbContextOptions<TangyuanDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Models.PostMetadata> PostMetadata { get; set; } = null;
        public DbSet<Models.User> User { get; set; } = null;
        public DbSet<Models.PostBody> PostBody { get; set; } = null;
        public DbSet<Models.Comment> Comment { get; set; } = null;
        public DbSet<Models.Notification> Notification { get; set; } = null;
        public DbSet<Models.Category> Category { get; set; } = null;
        public DbSet<Models.NewNotification> NewNotification { get; set; } = null;
    }
}
