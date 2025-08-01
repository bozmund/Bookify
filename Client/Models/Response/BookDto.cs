namespace Client.Models.Response;

public record BookDto
{
    public int Id { get; set; }
    public string BookId { get; set; }
    public string BestBookId { get; set; }
    public string WorkId { get; set; }
    public int BooksCount { get; set; }
    public string Isbn { get; set; }
    public string Isbn13 { get; set; }
    public string Authors { get; set; }
    public float? OriginalPublicationYear { get; set; }
    public string OriginalTitle { get; set; }
    public string Title { get; set; }
    public string LanguageCode { get; set; }
    public decimal AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int WorkRatingsCount { get; set; }
    public int WorkTextReviewsCount { get; set; }
    public int Ratings1 { get; set; }
    public int Ratings2 { get; set; }
    public int Ratings3 { get; set; }
    public int Ratings4 { get; set; }
    public int Ratings5 { get; set; }
    public string ImageUrl { get; set; }
    public string SmallImageUrl { get; set; }
}