using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class BookDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
    }
}