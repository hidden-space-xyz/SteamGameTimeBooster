namespace SteamGameTimeBooster.Helpers;

public static class ConsoleHelper
{
    public static void WriteColored(string text, ConsoleColor color)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write("  ");
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteColoredLine(string text, ConsoleColor color)
    {
        WriteColored(text + Environment.NewLine, color);
    }
}
