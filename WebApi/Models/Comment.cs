namespace WebApi.Models;

public class Comment
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserName { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}