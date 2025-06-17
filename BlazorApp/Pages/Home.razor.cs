using BlazorApp.Models;
using Client;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages;

public class HomeBase : ComponentBase
{
    [Inject] protected IBookClient Client { get; set; } = null!;
    [Inject] NavigationManager NavigationManager { get; set; }

    protected void NavigateToBook(int bookId)
    {
        NavigationManager.NavigateTo($"/book/{bookId}");
    }

    protected string NameOfTheBook { get; set; } = "Harry Potter";
    protected List<BookDto> Books { get; set; } = [];

    protected async Task SearchBook()
    {
        Books.Clear();
        var bookDtos = await Client.GetBookByNameAsync(NameOfTheBook);
        Books.AddRange(bookDtos);
    }
}