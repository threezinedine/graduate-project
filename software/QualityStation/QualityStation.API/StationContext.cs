using Microsoft.EntityFrameworkCore;
using QualityStation.API.Models;
using System.Security.Cryptography.X509Certificates;

namespace QualityStation.API
{
	public class StationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<AirQualityRecord> AirQualityRecords { get; set; }
        public DbSet<RecordAttribute> RecordAttributes { get; set; }

        public StationContext(DbContextOptions options)
	        : base(options)	
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Stations)
                .WithMany(e => e.Users)
                .UsingEntity<UserStation>();
        }
    }
}
