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
        base.OnModelCreating(builder);

        builder.Entity<Market>().ToTable("Market");
    }

    public DbSet<Market> MarketList {get; set;}
    public DbSet<Product> ProductList {get; set;}

    }