using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<WeddingCard> WeddingCards => Set<WeddingCard>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<CardImage> CardImages => Set<CardImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeddingCard>(entity =>
        {
            entity.HasIndex(e => e.SlugUrl).IsUnique();

            entity.HasMany(e => e.Images)
                .WithOne()
                .HasForeignKey(i => i.WeddingCardId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
