# Stellaris Steam Downloader

The Stellaris Steam Downloader is a tool that allows you to download mods for the game Stellaris from the Steam Workshop. This program utilizes SteamCMD, a command-line utility for Steam, to fetch the mod files based on the provided ModID or URL.

## Requirements

- **SteamCMD:** Make sure you have SteamCMD installed on your system. You can download it from the official [SteamCMD website](https://developer.valvesoftware.com/wiki/SteamCMD).

## Usage

1. Clone or download the repository to your local machine.

2. Open the program file `Program.cs` and update the following variables as per your system configuration:
   - `steamCmdPath`: Path to your SteamCMD executable.
   - `appId`: The desired AppID for Stellaris.

3. Build and run the program using your preferred C# compiler or development environment.

4. Follow the prompts in the console application:
   - Enter the ModID or URL of the mod you want to download.
   - The program will check if the ModID is valid and retrieve the mod name.
   - SteamCMD will be invoked to download the mod files from the Steam Workshop.
   - The downloaded files will be zipped and organized in a folder with the mod's ID.
   - A `.mod` file will be created with relevant mod information.
   - The mod folder and files will be moved to the appropriate locations.

## Notes

- The paths in the code (`steamCmdPath`, `outputFolderPath`, etc.) are set according to my system. You may need to modify them to match your own directory structure.

- This program relies on the availability and functionality of SteamCMD and the Steam Workshop webpages.