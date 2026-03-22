using Microsoft.EntityFrameworkCore;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<WeddingCard> WeddingCards { get; }
    DbSet<Template> Templates { get; }
    DbSet<User> Users { get; }
    DbSet<Guest> Guests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
