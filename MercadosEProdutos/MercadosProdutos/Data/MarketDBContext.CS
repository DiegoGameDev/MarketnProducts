using DBModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DBContext;

public class MarketDBContext : IdentityDbContext<User>
{
    public MarketDBContext(DbContextOptions<MarketDBContext> opt) : base (options: opt)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Market>().ToTable("Market");
        builder.Entity<MarketAssociated>().ToTable("MarketAssociated");

        builder.Entity<MarketAssociated>().HasOne(x => x.Market).WithMany(m => m.MarketAssociatedList)
        .HasForeignKey(x => x.MarketID);

        builder.Entity<MarketAssociated>().HasOne(x => x.User).WithMany(u => u.marketAssociateds)
        .HasForeignKey(x => x.UserID);
        base.OnModelCreating(builder);
    }

    public DbSet<Market> MarketList {get; set;}
    public DbSet<Product> ProductList {get; set;}
    public DbSet<MarketAssociated> MarketAssociatedList {get; set;}

    }