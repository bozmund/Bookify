using System.Net.Http.Json;
using System.Text.Json;
using BlazorApp.Models;
using Client;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages;

public partial class BookPage : ComponentBase
{
    [Parameter] public int Id { get; set; }

    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5000")
    };

    private List<int> _recommendedBookIds = [];
    protected List<BookDto> Books = [];
    protected BookDto? MainBook { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var byIdAsync = await BookClient.GetBooksByIdAsync([Id]);
        MainBook = byIdAsync.First();
        var json = JsonSerializer.Serialize(new { book_id = Id });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var httpResponseMessage = await HttpClient.PostAsync("recommend", content);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var responseJson = await httpResponseMessage.Content.ReadFromJsonAsync<JsonElement>();
            _recommendedBookIds = responseJson
                .GetProperty("recommendations")
                .EnumerateArray()
                .Select(rec => rec.GetProperty("book_id").GetInt32())
                .ToList();

            Books = await BookClient.GetBooksByIdAsync(_recommendedBookIds);
        }
        else
        {
            Console.WriteLine(
                $"Failed to fetch recommendations for book {Id}. Status code: {httpResponseMessage.StatusCode}");
        }
    }

    [Inject] private IBookClient BookClient { get; set; }
}