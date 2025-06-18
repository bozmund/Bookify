using BlazorApp.Models;
using Client;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages;

public class HomeBase : ComponentBase
{
    [Inject] protected IBookClient Client { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    protected List<BookDto> RecommendedBooks { get; } = [];
    protected string NameOfTheBook { get; set; } = "Harry Potter";
    protected void NavigateToBook(int bookId)
    {
        NavigationManager.NavigateTo($"/book/{bookId}");
    }
    protected async Task SearchBook()
    {
        RecommendedBooks.Clear();
        var bookDtos = await Client.GetBookByNameAsync(NameOfTheBook);
        RecommendedBooks.AddRange(bookDtos);
    }
}