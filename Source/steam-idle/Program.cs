using Steamworks;

namespace steam_idle;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args == null || args.Length == 0)
            return;

        if (!long.TryParse(args[0], out long appId))
            return;

        try
        {
            Environment.SetEnvironmentVariable("SteamAppId", appId.ToString());

            if (!SteamAPI.Init())
                return;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}", ex.GetType().FullName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmMain(appId));
    }
}