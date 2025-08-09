using System.Collections;
using System.Text.Json;
using BlazorWebApp.Components.Account;
using BlazorWebApp.Data;
using Client;
using Client.Models;
using Client.Models.Response;
using Client.Models.Values;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;

namespace BlazorWebApp.Components.Pages;

public partial class BookPage : ComponentBase
{
    [Parameter] public int Id { get; set; }
    [Inject] private IBookClient BookClient { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    
    private string? _currentUser;
    
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
    public int UserRating = 0;


    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        await LoadUserAsync();
        await LoadBookAsync();
        await LoadLikesAsync();
        await LoadCommentsAsync();
        await LoadRatingsAsync();
        IsLoading = false;
    }

    private async Task LoadLikesAsync()
    {
        if (_currentUser != null)
        {
            var likedBooksAsync = await BookClient.GetLikedBookAsync(_currentUser, Id);
            if (likedBooksAsync.IsSuccessful){
                var likedBooks = likedBooksAsync.Content;
                Read = likedBooks.Any(x => x.Type == LikedType.Read);
                ToBeRead = likedBooks.Any(x => x.Type == LikedType.ToRead);
            }
        }
        else
        {
            Read = false;
        }
    }

    public bool ToBeRead { get; set; }

    private async Task LoadUserAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _currentUser = authState.User.Identity?.Name;
    }

    private async Task LoadRatingsAsync()
    {
        if (_currentUser != null)
        {
            var response = await BookClient.GetRatingAsync(_currentUser, Id);
            if (response.IsSuccessful)
            {
                UserRating = response.Content;
            }
        }
    }

    private async Task LoadBookAsync()
    {
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

    private async Task AddBookAsReadAsync()
    {
        if (MainBook?.Id != null)
        {
            var likedBook = new LikedBook
            {
                UserName = _currentUser,
                BookId = MainBook.Id,
                Type = LikedType.Read
            };
            await BookClient.LikeBookAsync(likedBook);
            var likedBooksAsync = await BookClient.GetLikedBookAsync(_currentUser, MainBook.Id);
        
            Read = likedBooksAsync.IsSuccessful ? likedBooksAsync.Content.Any(x => x.Type == LikedType.Read) : false;
        }
    }
    
    private async Task RemoveBookFromReadAsync()
    {
        if (MainBook?.Id != null)
        {
            var likedBook = new LikedBook
            {
                UserName = _currentUser,
                BookId = MainBook.Id,
                Type = LikedType.Read
            };
            var unlikedBookAsync = await BookClient.UnlikeBook(likedBook);
        
            Read = unlikedBookAsync.IsSuccessful ? false : Read;
        }
    }

    public bool Read { get; set; }

    private async Task AddBookToWantToReadAsync()
    {
        if (MainBook?.Id != null)
        {
            var likedBook = new LikedBook
            {
                UserName = _currentUser,
                BookId = MainBook.Id,
                Type = LikedType.ToRead
            };
            await BookClient.LikeBookAsync(likedBook);
            var likedBooksAsync = await BookClient.GetLikedBookAsync(_currentUser, MainBook.Id);
        
            ToBeRead = likedBooksAsync.IsSuccessful ? likedBooksAsync.Content.Any(x => x.Type == LikedType.ToRead) : false;
        }
    }
    
    private async Task RemoveBookFromWantToReadAsync()
    {
        if (MainBook?.Id != null)
        {
            var likedBook = new LikedBook
            {
                UserName = _currentUser,
                BookId = MainBook.Id,
                Type = LikedType.ToRead
            };
            var unlikedBookAsync = await BookClient.UnlikeBook(likedBook);
        
            ToBeRead = unlikedBookAsync.IsSuccessful ? false : Read;
        }
    }

    private async Task Rate(int arg)
    {
        if (_currentUser != null)
        {
            await BookClient.RateAsync(_currentUser, MainBook?.Id ?? 0, arg);
            var ratingResponse = await BookClient.GetRatingAsync(_currentUser, MainBook?.Id ?? 0);
            if (ratingResponse.IsSuccessful)
            {
                UserRating = ratingResponse.Content;
            }
        }
    }
}