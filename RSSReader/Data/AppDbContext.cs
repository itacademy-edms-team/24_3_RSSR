using Microsoft.EntityFrameworkCore;
using RSSReader.Models;

namespace RSSReader.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
        public DbSet<Folder> Folders { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //feed
            modelBuilder.Entity<Feed>()
                        .HasKey(f => f.FeedId);

            modelBuilder.Entity<Feed>()
                .Property(f => f.FeedId).ValueGeneratedOnAdd(); 

            modelBuilder.Entity<Feed>()
                .HasMany(f => f.FeedItems)
                .WithOne(i => i.Feed)
                .OnDelete(DeleteBehavior.Cascade);

            //folder
            modelBuilder.Entity<Folder>()
            .HasKey(f => f.FolderId);

            modelBuilder.Entity<Folder>()
                .Property(f => f.FolderId).ValueGeneratedOnAdd();

            modelBuilder.Entity<Folder>()
                .HasMany(f => f.Feeds)
                .WithMany(f => f.Folders);

            //feedItem
            modelBuilder.Entity<FeedItem>()
            .HasKey(f => f.FeedItemId);

            modelBuilder.Entity<FeedItem>()
                .Property(f => f.FeedItemId).ValueGeneratedOnAdd();


        }
    }
}
