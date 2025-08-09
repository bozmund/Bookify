using Client.Models.Values;

namespace Client.Models.Request;

public record UserInfoDto(string UserName, LikedType Type);