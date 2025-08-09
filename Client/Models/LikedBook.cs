using Client.Models.Values;

namespace Client.Models;

public class LikedBook
{
    public int BookId { get; set; }
    public string UserName { get; set; }
    public LikedType Type { get; set; }
}
