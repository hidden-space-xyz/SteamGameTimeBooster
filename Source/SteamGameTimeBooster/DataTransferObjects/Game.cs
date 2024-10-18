namespace SteamGameTimeBooster.DataTransferObjects;

public sealed class GamesListDataModel
{
    public required List<Game> RgGames { get; set; }
}

public sealed class Game
{
    public required int AppId { get; set; }
    public required string Name { get; set; }
}