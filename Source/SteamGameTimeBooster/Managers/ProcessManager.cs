using SteamGameTimeBooster.DataTransferObjects;
using System.Diagnostics;
using static SteamGameTimeBooster.Helpers.ConsoleHelper;

namespace SteamGameTimeBooster.Managers
{
    internal sealed class ProcessManager
    {
        public static readonly ProcessManager Instance = new();

        private readonly List<Process> _runningProcesses = [];

        private ProcessManager() { }

        public async Task StartGameProcess(int gameId, List<Game> userGames, TimeSpan duration, CancellationToken token)
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

                lock (_runningProcesses) { _runningProcesses.Add(process); }

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
                lock (_runningProcesses) { _runningProcesses.Remove(process); }
                process.Dispose();
            }
        }

        public void KillAllProcesses()
        {
            lock (_runningProcesses)
            {
                foreach (Process process in _runningProcesses)
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        WriteColoredLine($"⏹️ Terminated process ID {process.Id}.", ConsoleColor.Magenta);
                    }
                }
                _runningProcesses.Clear();
            }
        }
    }
}
