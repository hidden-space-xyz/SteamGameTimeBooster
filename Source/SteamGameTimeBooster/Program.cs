using HtmlAgilityPack;
using SteamGameTimeBooster.DataTransferObjects;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace SteamGameTimeBooster;

internal static class Program
{
    private static string userName;
    private static string sessionId;
    private static string steamLoginSecure;

    private static readonly List<Process> runningProcesses = [];

    private static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (s, e) => KillAllProcesses();
        Console.CancelKeyPress += (s, e) => { e.Cancel = true; KillAllProcesses(); Environment.Exit(0); };
        Console.OutputEncoding = System.Text.Encoding.UTF8;

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

        LogIn();

        if (!IsLoggedIn())
        {
            WriteColoredLine("❌ Invalid user credentials.", ConsoleColor.Red);
            return;
        }

        List<GameDataModel> userGames = GetAllUserGames();
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
        IEnumerable<Task> tasks = pickedGames.Select(gameId => StartGameProcess(gameId, userGames, duration, cts.Token));

        await Task.WhenAll(tasks);

        Console.WriteLine();
        WriteColoredLine("✅ All processes have finished.", ConsoleColor.Green);
    }

    private static void DisplayGameList(List<GameDataModel> userGames)
    {
        Console.WriteLine();
        WriteColoredLine("🎮 Available Games:", ConsoleColor.Cyan);

        foreach (GameDataModel game in userGames)
            WriteColoredLine($"    [{game.AppId}] {game.Name}", ConsoleColor.White);
    }

    private static void LogIn()
    {
        while (true)
        {
            WriteColored("⌨️ Enter your steam username: ", ConsoleColor.Yellow);
            userName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userName))
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);

            else break;
        }

        while (true)
        {
            WriteColored("⌨️ Enter your steam sessionId: ", ConsoleColor.Yellow);
            sessionId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(sessionId))
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);

            else break;
        }

        while (true)
        {
            WriteColored("⌨️ Enter your steam steamLoginSecure: ", ConsoleColor.Yellow);
            steamLoginSecure = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(steamLoginSecure))
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);

            else break;
        }
    }

    private static List<int> GetUserGameSelection(List<GameDataModel> userGames)
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
            WriteColored("⌨️ Enter duration in hours:minutes (e.g., 1:30): ", ConsoleColor.Yellow);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);
                continue;
            }

            if (TimeSpan.TryParseExact(input, @"h\:mm", CultureInfo.InvariantCulture, out TimeSpan duration) && duration > TimeSpan.Zero)
                return duration;
            else
                WriteColoredLine("❌ Invalid duration format. Please enter the time in hours:minutes format (e.g., 1:30).", ConsoleColor.Red);
        }
    }

    private static async Task StartGameProcess(int gameId, List<GameDataModel> userGames, TimeSpan duration, CancellationToken token)
    {
        string appId = userGames.First(x => x.AppId == gameId).AppId.ToString();
        Process process = new()
        {
            StartInfo = new ProcessStartInfo("steam-idle.exe", appId)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        try
        {
            process.Start();

            lock (runningProcesses) { runningProcesses.Add(process); }

            WriteColoredLine($"▶️ Started process for game {appId}.", ConsoleColor.Green);

            await Task.Delay(duration, token);

            if (!process.HasExited)
            {
                process.Kill();

                Console.WriteLine();
                WriteColoredLine($"⏹️ Process for game {appId} has been terminated after {duration.TotalMinutes} minute(s).", ConsoleColor.Magenta);
            }
        }
        catch (Exception ex)
        {
            WriteColoredLine($"❌ Error with game {appId}: {ex.Message}.", ConsoleColor.Red);
        }
        finally
        {
            lock (runningProcesses) { runningProcesses.Remove(process); }
            process.Dispose();
        }
    }

    private static void KillAllProcesses()
    {
        lock (runningProcesses)
        {
            foreach (Process process in runningProcesses)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    WriteColoredLine($"⏹️ Terminated process ID {process.Id}.", ConsoleColor.Magenta);
                }
            }
            runningProcesses.Clear();
        }
    }

    private static void WriteColored(string text, ConsoleColor color)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write("  ");
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    private static void WriteColoredLine(string text, ConsoleColor color)
    {
        WriteColored(text + Environment.NewLine, color);
    }

    private static bool IsLoggedIn()
    {
        using HttpClient client = new(CreateSteamCookiesHandler());
        string response = client.GetStringAsync($"https://steamcommunity.com/id/{userName}").Result;

        HtmlDocument document = new();
        document.LoadHtml(response);

        return document.DocumentNode.SelectSingleNode("//div[@class=\"responsive_menu_user_area\"]") != null;
    }

    private static List<GameDataModel> GetAllUserGames()
    {
        using HttpClient client = new(CreateSteamCookiesHandler());
        string htmlResponse = client.GetStringAsync($"https://steamcommunity.com/id/{userName}/games?tab=all").Result;

        HtmlDocument doc = new();
        doc.LoadHtml(htmlResponse);

        HtmlNode? templateNode = doc.DocumentNode.SelectSingleNode("//template[@id='gameslist_config']");
        string jsonData = WebUtility.HtmlDecode(templateNode.GetAttributeValue("data-profile-gameslist", ""));

        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        GamesListDataModel? deserializedJson = JsonSerializer.Deserialize<GamesListDataModel>(jsonData, options);

        return deserializedJson?.RgGames.OrderBy(x => x.Name).ToList() ?? [];
    }

    private static HttpClientHandler CreateSteamCookiesHandler()
    {
        HttpClientHandler handler = new() { CookieContainer = new CookieContainer() };
        handler.CookieContainer.Add(new Cookie("sessionid", sessionId, "/", "steamcommunity.com"));
        handler.CookieContainer.Add(new Cookie("steamLoginSecure", steamLoginSecure, "/", "steamcommunity.com"));
        return handler;
    }
}
