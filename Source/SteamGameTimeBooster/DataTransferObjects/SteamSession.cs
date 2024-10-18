namespace SteamGameTimeBooster.DataTransferObjects;

public sealed class SteamSession
{
    public string UserName { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string SteamLoginSecure { get; set; } = string.Empty;

    public bool SimpleValidate() => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(SteamLoginSecure);
}
