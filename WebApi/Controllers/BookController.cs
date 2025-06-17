using BlazorApp.Models;
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

        var allBooks = await context.Books.AsNoTracking().ToListAsync();
        List<BookDto> result = [];
        result.AddRange(from book in allBooks
            where book.title.Contains(name, StringComparison.OrdinalIgnoreCase)
            select book.MapDto());

        return Ok(result);
    }
}