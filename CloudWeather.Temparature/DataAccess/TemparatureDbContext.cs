using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temparature.DataAccess
{
    public class TemparatureDbContext:DbContext
    {
        public TemparatureDbContext() { }

        public TemparatureDbContext(DbContextOptions opts) : base(opts) { }

        public DbSet<Temparature> Temparature { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }

        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temparature>(b => { b.ToTable("temparature"); });
        }
    }
}
