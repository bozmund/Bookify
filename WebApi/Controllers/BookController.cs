using Client.Models;
using Client.Models.Response;
using Client.Models.Values;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Controllers;

[Controller]
[Route("api/[controller]")]
public class BookController(BookDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetBookByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Name query parameter is required.");

        var allBooks = await context.Books.AsNoTracking().Where(x => x.IsDeleted != true).ToListAsync();
        List<BookDto> result = [];
        result.AddRange(from book in allBooks
            where book.title.Contains(name, StringComparison.OrdinalIgnoreCase)
            select book.MapDto());

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBook([FromQuery] int id)
    {
        if (id <= 0)
            return BadRequest("Invalid book ID.");

        var book = await context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        book.IsDeleted = true;
        context.Books.Update(book);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("books")]
    public async Task<ActionResult<List<BookDto>>> GetBookById([FromBody] List<int> recommendedBookIds)
    {
        if (recommendedBookIds.Count == 0)
            return BadRequest("Recommended book IDs are required.");

        var books = await context.Books
            .AsNoTracking()
            .Where(b => recommendedBookIds.Contains(b.id) && b.IsDeleted != true)
            .Select(b => b.MapDto())
            .ToListAsync();

        if (books.Count == 0)
            return NotFound("No books found for the provided IDs.");

        return Ok(books);
    }

    [HttpGet("liked/{userName}/{type}")]
    public async Task<ActionResult<List<BookDto>>> GetLikedBooks(string userName, LikedType type)  
    {
        if (string.IsNullOrWhiteSpace(userName))
            return BadRequest("User Name is required.");

        var likedBooks = context.LikedBooks
            .AsNoTracking()
            .Where(b => b.UserName == userName && b.Type == type)
            .Select(x => x.BookId);

        var books = await context.Books
            .AsNoTracking()
            .Where(b => likedBooks.Contains(b.id) && b.IsDeleted != true)
            .Select(b => b.MapDto())
            .ToListAsync();
        if (books.Count == 0)
            return NotFound("No liked books found for the user.");

        return Ok(books);
    }

    [HttpPost("like")]
    public async Task<IActionResult> LikeBook([FromBody] LikedBook likedBook)
    {
        if (string.IsNullOrWhiteSpace(likedBook.UserName) || likedBook.BookId <= 0)
            return BadRequest("Invalid liked book data.");

        var existingLike = await context.LikedBooks
            .FirstOrDefaultAsync(lb => lb.UserName == likedBook.UserName && lb.BookId == likedBook.BookId && lb.Type == likedBook.Type);

        if (existingLike != null)
            return Conflict("This book is already liked by the user.");

        context.LikedBooks.Add(likedBook);
        await context.SaveChangesAsync();

        return Ok("Book liked successfully.");
    }
    
    [HttpGet("liked/{userName}/book/{bookId}")]
    public async Task<ActionResult<IList<LikedBook>>> GetLikedBookAsync(string userName, int bookId)
    {
        if (string.IsNullOrWhiteSpace(userName) || bookId <= 0)
            return BadRequest("Invalid parameters.");

        var likedBook = await context.LikedBooks
            .AsNoTracking()
            .Where(b => b.UserName == userName && b.BookId == bookId)
            .ToListAsync();

        if (likedBook.Count == 0)
            return NotFound("Liked book not found.");

        return Ok(likedBook);
    }

    [HttpDelete("unlike")]
    public async Task<IActionResult> UnlikeBook([FromBody] LikedBook likedBook)
    {
        if (string.IsNullOrWhiteSpace(likedBook.UserName) || likedBook.BookId <= 0)
            return BadRequest("Invalid liked book data.");
        var existingLike = await context.LikedBooks
            .FirstOrDefaultAsync(lb => lb.UserName == likedBook.UserName && lb.BookId == likedBook.BookId && lb.Type == likedBook.Type);
        if (existingLike == null)
            return NotFound("This book is not liked by the user.");
        context.LikedBooks.Remove(existingLike);
        await context.SaveChangesAsync();
        return Ok("Book unliked successfully.");
    }

    [HttpPost("rate/{userName}/{bookId}")]
    public async Task<IActionResult> RateAsync(string userName, int bookId, [FromBody] int rating)
    {
        if (string.IsNullOrWhiteSpace(userName) || bookId <= 0 || rating < 1 || rating > 5)
            return BadRequest("Invalid parameters.");
        var book = await context.Books.FindAsync(bookId);
        if (book == null)
            return NotFound("Book not found.");
        var existingRating = await context.Ratings
            .FirstOrDefaultAsync(r => r.UserName == userName && r.BookId == bookId);
        if (existingRating != null)
        {
            return Conflict("This book has already been rated by the user.");
        }
        var newRating = new Rating
        {
            UserName = userName,
            BookId = bookId,
            Value = rating
        };
        context.Ratings.Add(newRating);
        await context.SaveChangesAsync();
        return Ok("Book rated successfully.");
    }

    [HttpGet("rating/{userName}/{bookId}")]
    public async Task<ActionResult<int>> GetRatingAsync(string userName, int bookId)
    {
        if (string.IsNullOrWhiteSpace(userName) || bookId <= 0)
            return BadRequest("Invalid parameters.");
        var book = await context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.id == bookId && b.IsDeleted != true);
        if (book == null)
            return NotFound("Book not found.");
        var rating = await context.Ratings
            .AsNoTracking()
            .Where(r => r.UserName == userName && r.BookId == bookId)
            .Select(r => r.Value)
            .FirstOrDefaultAsync();
        if (rating == 0)
            return NotFound("Rating not found for this user and book.");
        
        return Ok(rating);
    }
}