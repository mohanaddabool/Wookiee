using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Internal;
using Wookiee.Model.Entities;

namespace Wookiee.Repository.Context;

public class WookieeContext: IdentityDbContext<IdentityUser>
{
    public WookieeContext(DbContextOptions<WookieeContext> options): base(options)
    {
        
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(6, 2);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IdentityUser>().Property(t => t.UserName).IsRequired();
        builder.Entity<IdentityUser>().Property(t => t.Email).IsRequired();
        builder.Entity<IdentityUser>().Property(t => t.PasswordHash).IsRequired();
        builder.Entity<IdentityUser>().Property(t => t.SecurityStamp).IsRequired();
        builder.Entity<IdentityUser>().Property(t => t.ConcurrencyStamp).IsRequired();
        builder.Entity<User>().HasIndex(t => t.AuthorPseudonym).IsUnique();
        base.OnModelCreating(builder);
        builder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable(name: "Users");
        });
    }

    public DbSet<User> User { get; set; }
    public DbSet<Book> Books { get; set; }
}