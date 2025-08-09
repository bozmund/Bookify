using Client.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data;

public class BookDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<LikedBook> LikedBooks { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LikedBook>()
            .HasKey(lb => new { lb.BookId, lb.UserName, lb.Type });
        
        modelBuilder.Entity<Rating>()
            .HasKey(r => new { r.BookId, r.UserName });
    }
}