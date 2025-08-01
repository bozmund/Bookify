using System.Collections;
using System.Text.Json;
using BlazorWebApp.Components.Account;
using BlazorWebApp.Data;
using Client;
using Client.Models.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebApp.Components.Pages;

public partial class BookPage : ComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private IBookClient BookClient { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    
    private ApplicationUser CurrentUser = default!;
    
    private static readonly HttpClient RecommendationClient = new()
    {
        BaseAddress = new Uri("http://localhost:5000")
    };

    private static readonly HttpClient CommentClient = new()
    {
        BaseAddress = new Uri("http://localhost:5022/api/comments/")
    };

    private bool IsLoading { get; set; }
    private IList<CommentDto> Comments = [];
    private List<int> _recommendedBookIds = [];
    private List<BookDto> RecommendedBooks = [];
    private BookDto? MainBook { get; set; }
    private string CommentText { get; set; } = string.Empty;


    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        var byIdAsync = await BookClient.GetBooksByIdAsync([Id]);
        MainBook = byIdAsync.First();
        var json = JsonSerializer.Serialize(new { book_id = Id });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        try
        {
            var httpResponseMessage = await RecommendationClient.PostAsync("recommend", content);
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
        }
        catch (Exception)
        {
            // ignored
        }

        await LoadCommentsAsync();
        IsLoading = false;
    }

    private async Task LoadCommentsAsync()
    {
        var response = await CommentClient.GetAsync($"/api/comments/{MainBook?.Id}");
        if (response.IsSuccessStatusCode)
        {
            var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
            Comments = comments ?? [];
        }
        else
        {
            Comments = [];
        }
    }

    private async Task PostCommentAsync()
    {
        var user = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        
        if (user.User.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(CommentText))
        {
            var commentDto = new Client.Models.Request.CommentDto(Id, user.User.Identity.Name, CommentText);
            var json = JsonSerializer.Serialize(commentDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await CommentClient.PostAsync("book", content);
            if (!response.IsSuccessStatusCode)
            {
                // Handle error (e.g., show a message to the user)
                Console.WriteLine("Failed to post comment.");
                return;
            }
        }
        //refresh comments after posting
        await LoadCommentsAsync();
        CommentText = string.Empty;
    }
}