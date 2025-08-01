namespace Client.Models.Response;

public record CommentDto(string UserName, string Text, DateTime CreatedAt);