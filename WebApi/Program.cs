using System.Globalization;
using Client.Models.Response;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserve property names as they are
    options.JsonSerializerOptions.DictionaryKeyPolicy = null; // Preserve dictionary keys as they are
});
services.AddDbContext<BookDbContext>(options => options.UseSqlite("Data Source=Books.db"));
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

services.AddCors(x =>
    x.AddPolicy("AllowAll", policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));
var app = builder.Build();
var commentsGroup = app.MapGroup("/api/comments");
commentsGroup.MapGet("/{bookId:int}", async (int bookId, BookDbContext db) =>
{
    var comments = await db.Comments
        .Where(c => c.BookId == bookId)
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync();
    return comments.Count == 0
        ? Results.NotFound("No comments found for this book.")
        : Results.Ok(comments.Select(x => new CommentDto(x.UserName, x.Text, x.CreatedAt)));
});

commentsGroup.MapPost("/book", async (Comment comment, BookDbContext db) =>
{
    comment.CreatedAt = DateTime.UtcNow;
    db.Comments.Add(comment);
    await db.SaveChangesAsync();
    return Results.Ok(comment);
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();

    var seed = false;

    try
    {
        seed = !await db.Books.AnyAsync();
    }
    catch
    {
        db.Database.Migrate();
        seed = true;
    }

    if (seed)
    {
        var csvPath = Path.Combine(AppContext.BaseDirectory, "books.csv");

        if (!File.Exists(csvPath))
        {
            throw new FileNotFoundException(
                $"Required CSV file for seeding books not found at '{csvPath}'. Ensure the file is present before starting the application.",
                csvPath
            );
        }

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null
            }
        );

        var books = csv.GetRecords<Book>();
        db.Books.AddRange(books);
        db.SaveChanges();
    }
}


app.UseCors("AllowAll");
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();