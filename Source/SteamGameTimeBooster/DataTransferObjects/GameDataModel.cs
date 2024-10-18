namespace SteamGameTimeBooster.DataTransferObjects;

public class GamesListDataModel
{
    public required List<GameDataModel> RgGames { get; set; }
}

public class GameDataModel
{
    public required int AppId { get; set; }
    public required string Name { get; set; }
}