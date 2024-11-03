using Microsoft.EntityFrameworkCore;

namespace MasterData.Domain
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new VendorConfiguration());

            // TODO: rest of the configs
        }
    }
}
