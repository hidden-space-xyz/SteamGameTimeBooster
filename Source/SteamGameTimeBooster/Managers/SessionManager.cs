using HtmlAgilityPack;
using SteamGameTimeBooster.DataTransferObjects;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static SteamGameTimeBooster.Helpers.ConsoleHelper;

namespace SteamGameTimeBooster.Managers;

internal sealed class SessionManager
{
    public static readonly SessionManager Instance = new();

    private readonly SteamSession _session = new();

    private SessionManager()
    {
        if (File.Exists(EncryptionConstants.EncryptedDataFilePath))
        {
            try
            {
                byte[] encryptedData = File.ReadAllBytes(EncryptionConstants.EncryptedDataFilePath);
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);

                string jsonString = Encoding.UTF8.GetString(decryptedData);
                _session = JsonSerializer.Deserialize<SteamSession>(jsonString);
            }
            catch { WriteColoredLine("⚠️ Failed to load session data. Please enter your credentials.", ConsoleColor.Yellow); }
        }
    }

    public void LogIn()
    {
        if (_session.SimpleValidate())
        {
            WriteColoredLine("✅ Session data loaded successfully.", ConsoleColor.Green);
            return;
        }

        AskForUserName();
        AskForSessionId();
        AskForLoginSecure();
        AskIfKeepLogedIn();
    }

    public bool IsLoggedIn()
    {
        using HttpClient client = new(CreateSteamCookiesHandler());
        string response = client.GetStringAsync($"https://steamcommunity.com/id/{_session.UserName}").Result;

        HtmlDocument document = new();
        document.LoadHtml(response);

        bool loggedIn = document.DocumentNode.SelectSingleNode("//div[@class=\"responsive_menu_user_area\"]") != null;

        if (!loggedIn)
        {
            WriteColoredLine("❌ Invalid user credentials.", ConsoleColor.Red);
            Console.WriteLine();
        }

        return loggedIn;
    }

    public List<Game> GetAllUserGames()
    {
        using HttpClient client = new(CreateSteamCookiesHandler());
        string htmlResponse = client.GetStringAsync($"https://steamcommunity.com/id/{_session.UserName}/games?tab=all").Result;

        HtmlDocument doc = new();
        doc.LoadHtml(htmlResponse);

        HtmlNode? templateNode = doc.DocumentNode.SelectSingleNode("//template[@id='gameslist_config']");
        string jsonData = WebUtility.HtmlDecode(templateNode.GetAttributeValue("data-profile-gameslist", ""));

        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        GamesListDataModel? deserializedJson = JsonSerializer.Deserialize<GamesListDataModel>(jsonData, options);

        return deserializedJson?.RgGames.OrderBy(x => x.Name).ToList() ?? [];
    }

    private HttpClientHandler CreateSteamCookiesHandler()
    {
        HttpClientHandler handler = new() { CookieContainer = new CookieContainer() };
        handler.CookieContainer.Add(new Cookie("sessionid", _session.SessionId, "/", "steamcommunity.com"));
        handler.CookieContainer.Add(new Cookie("steamLoginSecure", _session.SteamLoginSecure, "/", "steamcommunity.com"));
        return handler;
    }

    private void AskIfKeepLogedIn()
    {
        WriteColored("💾 Do you want to keep the session open for next time? (yes/no): ", ConsoleColor.Cyan);
        string response = Console.ReadLine();

        if (response.Trim().ToLower() is "yes" or "y")
        {
            string jsonString = JsonSerializer.Serialize(_session);

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(jsonString);
            byte[] encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(EncryptionConstants.EncryptedDataFilePath, encryptedData);

            WriteColoredLine("✅ Session data saved successfully.", ConsoleColor.Green);
        }
    }

    private void AskForLoginSecure()
    {
        while (true)
        {
            WriteColored("⌨️ Enter your steam steamLoginSecure: (type 'help' if you don't know how to get it) ", ConsoleColor.Yellow);
            _session.SteamLoginSecure = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(_session.SteamLoginSecure))
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);

            else if (_session.SteamLoginSecure == "help")
            {
                WriteColoredLine("ℹ️ Login to the official Steam Community website (https://steamcommunity.com/) in your web browser.", ConsoleColor.DarkYellow);
                WriteColoredLine("ℹ️ Press F12 > Application / Storage > Cookies > Select the Steam website > Copy the value of steamLoginSecure cookie.", ConsoleColor.DarkYellow);
                Console.WriteLine();
            }
            else break;
        }
    }

    private void AskForSessionId()
    {
        while (true)
        {
            WriteColored("⌨️ Enter your steam sessionId: (type 'help' if you don't know how to get it) ", ConsoleColor.Yellow);
            _session.SessionId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(_session.SessionId))
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);

            else if (_session.SessionId == "help")
            {
                WriteColoredLine("ℹ️ Login to the official Steam Community website (https://steamcommunity.com/) in your web browser.", ConsoleColor.DarkYellow);
                WriteColoredLine("ℹ️ Press F12 > Application/Storage > Cookies > Select the Steam website > Copy the value of sessionId cookie.", ConsoleColor.DarkYellow);
                Console.WriteLine();
            }
            else break;
        }
    }

    private void AskForUserName()
    {
        while (true)
        {
            WriteColored("⌨️ Enter your steam username: (type 'help' if you don't know how to get it) ", ConsoleColor.Yellow);
            _session.UserName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(_session.UserName))
                WriteColoredLine("❌ Input cannot be empty. Please try again.", ConsoleColor.Red);

            else if (_session.UserName == "help")
            {
                WriteColoredLine("ℹ️ Just enter the name you normally log in with.", ConsoleColor.DarkYellow);
                Console.WriteLine();
            }
            else break;
        }
    }
}
