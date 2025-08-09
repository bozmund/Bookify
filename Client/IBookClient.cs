using Client.Models;
using Client.Models.Response;
using Client.Models.Values;
using Refit;

namespace Client;

public interface IBookClient
{
    [Get("/api/book")]
    Task<List<BookDto>> GetBookByNameAsync([Query] string name);

    [Delete("/api/book")]
    Task DeleteBookAsync([Query] int id);

    [Post("/api/book/books")]
    Task<List<BookDto>> GetBooksByIdAsync([Body] List<int> recommendedBookIds);

    [Get("/api/book/liked/{userName}/{type}")]
    Task<IApiResponse<List<BookDto>>> GetLikedBooksAsync([Query] string userName, [Query] LikedType type);

    [Post("/api/book/like")]
    Task<IApiResponse> LikeBookAsync([Body] LikedBook likedBook);

    [Get("/api/book/liked/{userName}/book/{bookId}")]
    Task<IApiResponse<List<LikedBook>>> GetLikedBookAsync(string userName, int bookId);

    [Delete("/api/book/unlike")]
    Task<IApiResponse> UnlikeBook([Body] LikedBook likedBook);

    [Post("/api/book/rate/{userName}/{bookId}")]
    Task RateAsync([Query] string userName, [Query] int bookId, [Body] int rating);
    
    [Get("/api/book/rating/{userName}/{bookId}")]
    Task<IApiResponse<int>> GetRatingAsync([Query] string userName, [Query] int bookId);
}