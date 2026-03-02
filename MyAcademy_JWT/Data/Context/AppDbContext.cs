using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany()
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Song>()
                .HasOne(s => s.Album)
                .WithMany()
                .HasForeignKey(s => s.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed
            builder.Entity<Package>().HasData(
                new Package { Id = 1, Name = "Elite", MinLevel = 1 },
                new Package { Id = 2, Name = "Premium", MinLevel = 2 },
                new Package { Id = 3, Name = "Gold", MinLevel = 3 },
                new Package { Id = 4, Name = "Standard", MinLevel = 4 },
                new Package { Id = 5, Name = "Basic", MinLevel = 5 },
                new Package { Id = 6, Name = "Free", MinLevel = 6 }
            );
        }
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<UserSongHistory> UserSongHistories => Set<UserSongHistory>();
    }
}
