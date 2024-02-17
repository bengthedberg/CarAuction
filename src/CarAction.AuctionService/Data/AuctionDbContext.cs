using CarAction.AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAction.AuctionService.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Auction> Auctions { get; set; } = null!;
}
