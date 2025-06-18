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
    protected List<BookDto> RecommendedBooks = [];
    protected BookDto? MainBook { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        var byIdAsync = await BookClient.GetBooksByIdAsync([Id]);
        MainBook = byIdAsync.First();
        var json = JsonSerializer.Serialize(new { book_id = Id });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        try
        {
            var httpResponseMessage = await HttpClient.PostAsync("recommend", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseJson = await httpResponseMessage.Content.ReadFromJsonAsync<JsonElement>();
                _recommendedBookIds = responseJson
                    .GetProperty("recommendations")
                    .EnumerateArray()
                    .Select(rec => rec.GetProperty("book_id").GetInt32())
                    .ToList();

                RecommendedBooks = await BookClient.GetBooksByIdAsync(_recommendedBookIds);
            }
            //do nothing, no recommendations;
        }
        catch (Exception)
        {
            // Handle or log the exception as needed
        } //do nothing, no recommendations;
        IsLoading = false;
    }

    [Inject] private IBookClient BookClient { get; set; }
    public bool IsLoading { get; set; }
}