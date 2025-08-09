namespace Client.Models;

public class Rating
{
    public int BookId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Value { get; set; }
}