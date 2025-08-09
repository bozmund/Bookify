using Client;
using Client.Models.Response;
using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Pages;

public class HomeBase : ComponentBase
{
    [Inject] protected IBookClient Client { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    protected List<BookDto> Books { get; } = [];
    protected string NameOfTheBook { get; set; } = "Harry Potter";
    protected void NavigateToBook(int bookId)
    {
        NavigationManager.NavigateTo($"/book/{bookId}");
    }
    protected async Task SearchBook()
    {
        Books.Clear();
        var bookDtos = await Client.GetBookByNameAsync(NameOfTheBook);
        Books.AddRange(bookDtos);
    }
}