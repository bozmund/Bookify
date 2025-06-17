using BlazorApp.Models;

public class Book
{
    public int id { get; set; }
    public string book_id { get; set; }
    public string best_book_id { get; set; }
    public string work_id { get; set; }
    public int books_count { get; set; }
    public string isbn { get; set; }
    public string isbn13 { get; set; }
    public string authors { get; set; }
    public float? original_publication_year { get; set; }
    public string original_title { get; set; }
    public string title { get; set; }
    public string language_code { get; set; }
    public decimal average_rating { get; set; }
    public int ratings_count { get; set; }
    public int work_ratings_count { get; set; }
    public int work_text_reviews_count { get; set; }
    public int ratings_1 { get; set; }
    public int ratings_2 { get; set; }
    public int ratings_3 { get; set; }
    public int ratings_4 { get; set; }
    public int ratings_5 { get; set; }
    public string image_url { get; set; }
    public string small_image_url { get; set; }
    public bool IsDeleted { get; set; } = false;

    public BookDto MapDto()
    {
        return new BookDto
        {
            Id = id,
            BookId = book_id,
            BestBookId = best_book_id,
            WorkId = work_id,
            BooksCount = books_count,
            Isbn = isbn,
            Isbn13 = isbn13,
            Authors = authors,
            OriginalPublicationYear = original_publication_year,
            OriginalTitle = original_title,
            Title = title,
            LanguageCode = language_code,
            AverageRating = average_rating,
            RatingsCount = ratings_count,
            WorkRatingsCount = work_ratings_count,
            WorkTextReviewsCount = work_text_reviews_count,
            Ratings1 = ratings_1,
            Ratings2 = ratings_2,
            Ratings3 = ratings_3,
            Ratings4 = ratings_4,
            Ratings5 = ratings_5,
            ImageUrl = image_url,
            SmallImageUrl = small_image_url
        };
    }
}