using Client.Models.Response;
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
    public async Task<ActionResult<List<BookDto>>> GetBookById([FromBody]List<int> recommendedBookIds)
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
}