using BlazorApp.Models;
using Refit;

namespace Client;

public interface IBookClient
{
    [Get("/api/book")]
    Task<List<BookDto>> GetBookByNameAsync([Query] string name);
}