
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MapOfActivitiesAPI.Models
{
    public class MapOfActivitiesAPIContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Event> Types { get; set; }

        public MapOfActivitiesAPIContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>(entity =>
            {
                entity.HasOne(e => e.Type)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.TypeId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
