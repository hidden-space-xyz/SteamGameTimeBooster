using SteamGameTimeBooster.DataTransferObjects;
using SteamGameTimeBooster.Managers;
using System.Diagnostics;
using System.Globalization;
using static SteamGameTimeBooster.Helpers.ConsoleHelper;

namespace SteamGameTimeBooster;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WindowWidth = 140;
        Console.WindowHeight = 40;

        AppDomain.CurrentDomain.ProcessExit += (s, e) => ProcessManager.Instance.KillAllProcesses();
        Console.CancelKeyPress += (s, e) => { e.Cancel = true; ProcessManager.Instance.KillAllProcesses(); Environment.Exit(0); };

        Console.WriteLine();
        WriteColoredLine(" ██████╗  █████╗ ███╗   ███╗███████╗    ████████╗██╗███╗   ███╗███████╗    ██████╗  ██████╗  ██████╗ ███████╗████████╗███████╗██████╗ ", ConsoleColor.Magenta);
        WriteColoredLine("██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ╚══██╔══╝██║████╗ ████║██╔════╝    ██╔══██╗██╔═══██╗██╔═══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗", ConsoleColor.Magenta);
        WriteColoredLine("██║  ███╗███████║██╔████╔██║█████╗         ██║   ██║██╔████╔██║█████╗      ██████╔╝██║   ██║██║   ██║███████╗   ██║   █████╗  ██████╔╝", ConsoleColor.Magenta);
        WriteColoredLine("██║   ██║██╔══██║██║╚██╔╝██║██╔══╝         ██║   ██║██║╚██╔╝██║██╔══╝      ██╔══██╗██║   ██║██║   ██║╚════██║   ██║   ██╔══╝  ██╔══██╗", ConsoleColor.DarkMagenta);
        WriteColoredLine("╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗       ██║   ██║██║ ╚═╝ ██║███████╗    ██████╔╝╚██████╔╝╚██████╔╝███████║   ██║   ███████╗██║  ██║", ConsoleColor.DarkMagenta);
        WriteColoredLine(" ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝       ╚═╝   ╚═╝╚═╝     ╚═╝╚══════╝    ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝", ConsoleColor.DarkMagenta);
        Console.WriteLine();

        if (Process.GetProcessesByName("steam").Length == 0)
        {
            WriteColoredLine("❌ Steam is not running.", ConsoleColor.Red);
            return;
        }

        do SessionManager.Instance.LogIn();
        while (!SessionManager.Instance.IsLoggedIn());

        List<Game> userGames = SessionManager.Instance.GetAllUserGames();
        if (userGames == null || userGames.Count == 0)
        {
            WriteColoredLine("⚠️ No games found for the user.", ConsoleColor.Yellow);
            return;
        }

        DisplayGameList(userGames);
        List<int> pickedGames = GetUserGameSelection(userGames);

        TimeSpan duration = GetDurationFromUser();

        Console.WriteLine();
        WriteColoredLine($"⏳ Starting processes for selected games for {duration.TotalMinutes} minutes.", ConsoleColor.Cyan);
        Console.WriteLine();

        CancellationTokenSource cts = new();
        IEnumerable<Task> tasks = pickedGames.Select(gameId => ProcessManager.Instance.StartGameProcess(gameId, userGames, duration, cts.Token));

        await Task.WhenAll(tasks);

        Console.WriteLine();
        WriteColoredLine("✅ All processes have finished.", ConsoleColor.Green);
    }

    private static void DisplayGameList(List<Game> userGames)
    {
        Console.WriteLine();
        WriteColoredLine("🎮 Available Games:", ConsoleColor.Cyan);

        foreach (Game game in userGames)
            WriteColoredLine($"    [{game.AppId}] {game.Name}", ConsoleColor.White);
    }

    private static List<int> GetUserGameSelection(List<Game> userGames)
    {
        while (true)
        {
            WriteColored("⌨️ Enter the game IDs separated by commas (e.g., 990080, 550, 812140): ", ConsoleColor.Yellow);
            string? input = Console.ReadLine();

            try
            {
                List<int> pickedGames = input.Split(',').Select(id => int.Parse(id.Trim())).ToList();

                if (pickedGames.TrueForAll(id => userGames.Exists(g => g.AppId == id)))
                    return pickedGames;
                else
                    WriteColoredLine("❌ Invalid game ID(s). Please try again.", ConsoleColor.Red);
            }
            catch
            {
                WriteColoredLine("❌ Invalid input format. Please enter numbers separated by commas.", ConsoleColor.Red);
            }
        }
    }

    private static TimeSpan GetDurationFromUser()
    {
        while (true)
        {
            WriteColored("⌨️ Enter duration in hours:minutes (e.g., 01:30): ", ConsoleColor.Yellow);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);
                continue;
            }

            if (TimeSpan.TryParseExact(input, @"h\:mm", CultureInfo.InvariantCulture, out TimeSpan duration) && duration > TimeSpan.Zero)
                return duration;
            else
                WriteColoredLine("❌ Invalid duration format.", ConsoleColor.Red);
        }
    }
}
