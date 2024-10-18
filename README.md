# 🚀 SteamGameTimeBooster

**SteamGameTimeBooster** is a tool designed to automate the simulation of playtime in Steam titles. With this utility, you can simulate that several games are running, which increases the recorded playtime on your account.

## ✨ Features
- 🎮 **Simulate Running Games on Steam**: Easily simulate that one or multiple games are running on your Steam account to accumulate playtime. This is perfect for leveling up badges or showcasing playtime on your profile.
- 📜 **Multiple Game Selection**: Select multiple games from your Steam library to run simultaneously. Accumulate playtime on several games at once without the hassle of launching each game individually.
- ⏳ **Customizable Duration**: Set the exact duration you want the games to appear active. Whether it's for a few minutes or several hours, you have full control over the simulation time.
- ✅ **Automatic Closure**: Processes automatically terminate when the specified time is reached. No need to manually stop the simulation—the application handles it seamlessly.
- ⚙️ **Optimized Performance**: The application's performance is fully optimized, ensuring that resource consumption is negligible. You can run it in the background without any impact on your system's performance.
- 🚫 **No Game Installation Required**: You don't need to have the games installed on your computer—only the Steam application is required. This saves disk space and simplifies the process of simulating playtime.
- ⏱️ **Coming Soon:** More functionalities are on the way! I'm actively working on adding new features to enhance your experience. Stay tuned for future updates.

## 📋 Requirements
- Own a Steam account with **at least one game** in your library.
- Have Steam **installed and running** on your system.

## 🕹 Usage

1. Run `SteamGameTimeBooster.exe`.
2. Follow the on-screen instructions to enter your `userName`, `sessionId`, and `steamLoginSecure`:
   - Open your browser and log in to [Steam Community](https://steamcommunity.com/).
   - Press **F12** to open the developer tools.
   - Go to the **Application/Storage** tab > **Cookies**.
   - Find and copy the values of the `sessionid` and `steamLoginSecure` cookies.
3. Once logged in, a list of the games in your account will be displayed.
4. Enter the **IDs** of the games you want to run, separated by commas.
5. Indicate the desired duration in `hh:mm` (hours:minutes) format.
6. The program will start the simulated games for the time you have defined.
7. Once the time is up, the processes will close automatically.

## 📝 Example of Use

```
 ██████╗  █████╗ ███╗   ███╗███████╗    ████████╗██╗███╗   ███╗███████╗    ██████╗  ██████╗  ██████╗ ███████╗████████╗███████╗██████╗   
██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ╚══██╔══╝██║████╗ ████║██╔════╝    ██╔══██╗██╔═══██╗██╔═══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗  
██║  ███╗███████║██╔████╔██║█████╗         ██║   ██║██╔████╔██║█████╗      ██████╔╝██║   ██║██║   ██║███████╗   ██║   █████╗  ██████╔╝  
██║   ██║██╔══██║██║╚██╔╝██║██╔══╝         ██║   ██║██║╚██╔╝██║██╔══╝      ██╔══██╗██║   ██║██║   ██║╚════██║   ██║   ██╔══╝  ██╔══██╗  
╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗       ██║   ██║██║ ╚═╝ ██║███████╗    ██████╔╝╚██████╔╝╚██████╔╝███████║   ██║   ███████╗██║  ██║  
 ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝       ╚═╝   ╚═╝╚═╝     ╚═╝╚══════╝    ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝  

⌨️ Enter your steam username: mySteamUser
⌨️ Enter your steam sessionId: 123abc456def789ghi
⌨️ Enter your steam steamLoginSecure: abcdefg1234567890

🎮 Available Games:
    [1000] Dota 2
    [2000] Counter-Strike: Global Offensive
    [3000] Garry's Mod

⌨️ Enter the game IDs separated by commas (e.g., 570, 4000): 1000, 3000

⌨️ Enter duration in hours:minutes (e.g., 01:30): 02:00

⏳ Starting processes for selected games for 120 minutes.

⏹️ Process for game {appId} has been terminated after {duration.TotalMinutes} minute(s).

```

## ⚠️ Warnings

- **SteamGameTimeBooster** is a tool created **for educational purposes**. I am not responsible for the misuse of this application.
- Although to date **no cases of bans have been recorded** related to the use of tools like this, you should use it at your own risk.
- This repository exists with the purpose of showing how one can interact with Steam processes and simulate game sessions.

## 👨‍💻 Credits

- SteamGameTimeBooster was created by [Hidden Space](https://github.com/hidden-space-xyz), based on the original code created by [JonasNilson](https://github.com/JonasNilson), which in turn is based on the version by [jshackles](https://github.com/jshackles).

## 📝 License

This project is licensed under the [GNU General Public License v2.0](LICENSE).
