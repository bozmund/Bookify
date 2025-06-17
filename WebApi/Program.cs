using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;
services.AddControllers();
services.AddDbContext<BookDbContext>(options => options.UseSqlite("Data Source=Books.db"));
services.AddCors(x => x.AddPolicy("AllowAll", policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();
    db.Database.Migrate();

    if (!db.Books.Any())
    {
        using var reader = new StreamReader("books.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var books = csv.GetRecords<Book>();
        db.Books.AddRange(books);
        db.SaveChanges();
    }
}
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();