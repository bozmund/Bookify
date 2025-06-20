﻿using BlazorApp.Models;
using Refit;

namespace Client;

public interface IBookClient
{
    [Get("/api/book")]
    Task<List<BookDto>> GetBookByNameAsync([Query] string name);
    [Delete("/api/book")]
    Task DeleteBookAsync([Query] int id);
    [Post("/api/book/books")]
    Task<List<BookDto>> GetBooksByIdAsync([Body]List<int> recommendedBookIds);
}