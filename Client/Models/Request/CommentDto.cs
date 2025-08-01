namespace Client.Models.Request;

public record CommentDto(int BookId, string UserName, string Text);