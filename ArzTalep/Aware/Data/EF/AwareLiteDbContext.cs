using Aware.Model;
using Aware.Util.Lookup;
using Microsoft.EntityFrameworkCore;

namespace Aware.Data
{
    public class AwareLiteDbContext<T> : DbContext where T : DbContext
    {
        public AwareLiteDbContext(DbContextOptions<T> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Lookup> Lookup { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
