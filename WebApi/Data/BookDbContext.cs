using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class BookDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}