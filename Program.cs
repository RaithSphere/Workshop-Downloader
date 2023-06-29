using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using Newtonsoft.Json;

class Program
{
    static void Main()
    {
        string steamCmdPath = "C:\\steam\\steamcmd.exe"; // Path to your SteamCMD executable
        string appId = "281990"; // Replace with the desired AppID

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
  _____ _       _ _            _     _____                      _                 _           
 / ____| |     | | |          (_)   |  __ \                    | |               | |          
| (___ | |_ ___| | | __ _ _ __ _ ___| |  | | _____      ___ __ | | ___   __ _  __| | ___ _ __ 
 \___ \| __/ _ \ | |/ _` | '__| / __| |  | |/ _ \ \ /\ / / '_ \| |/ _ \ / _` |/ _` |/ _ \ '__|
 ____) | ||  __/ | | (_| | |  | \__ \ |__| | (_) \ V  V /| | | | | (_) | (_| | (_| |  __/ |   
|_____/ \__\___|_|_|\__,_|_|  |_|___/_____/ \___/ \_/\_/ |_| |_|_|\___/ \__,_|\__,_|\___|_|   
                                                                                              
");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Welcome to The Stellaris Steam Downloader");
        Console.WriteLine("Created by: Raith");
        Console.WriteLine();

        Console.WriteLine("The Stellaris Steam Downloader is a tool that allows you to download mods for the game Stellaris from the Steam Workshop.");
        Console.WriteLine("Simply enter the ModID or the URL of the mod, and the program will download the mod files for you.");
        Console.WriteLine("The downloaded files will be zipped and organized in a folder with the mod's ID.");
        Console.WriteLine();


        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Enter the ModID or URL:\n\n ");
        string modInput = Console.ReadLine();

        string modId;
        if (modInput.StartsWith("https://steamcommunity.com/sharedfiles/filedetails/?id="))
        {
            // Extract the ModID from the URL
            modId = modInput.Substring(modInput.LastIndexOf('=') + 1);
        }
        else
        {
            modId = modInput;
        }

        // Check if the ModID is valid
        bool isValid = IsModIDValid(modId);
        if (!isValid)
        {
            Console.WriteLine("Invalid ModID.");
            Environment.Exit(0);
        }

        string modName = GetModName(modId);
        Console.WriteLine($"Downloading mod: {modName}");


        string outputFolderPath = $"E:\\Raith\\SteamDownloader\\steam\\steamapps\\workshop\\content\\{appId}\\{modId}";

        // Start the SteamCMD process with arguments
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = steamCmdPath;
        startInfo.Arguments = "+login anonymous +workshop_download_item " + appId + " " + modId + " +quit";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;

        Process steamCmdProcess = new Process();
        steamCmdProcess.StartInfo = startInfo;
        steamCmdProcess.Start();

        // Read the output of the SteamCMD process
        string output = steamCmdProcess.StandardOutput.ReadToEnd();

        steamCmdProcess.WaitForExit();
        steamCmdProcess.Close();

        Console.WriteLine("SteamCMD operation completed.");
        Console.WriteLine("Output:");
        Console.WriteLine(output);

        // Create a zip file of the downloaded contents
        string zipFilePath = $"E:\\Raith\\SteamDownloader\\steam\\steamapps\\workshop\\content\\{appId}\\{modId}.zip";
        // Check if the zip file already exists and delete it if it does
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
            Console.WriteLine("Existing zip file deleted.");
        }

        // Create a zip file of the downloaded contents
        ZipFile.CreateFromDirectory(outputFolderPath, zipFilePath);

        Console.WriteLine("Contents zipped successfully.");

        // Delete the output folder
        Directory.Delete(outputFolderPath, true);

        Console.WriteLine("Output folder deleted successfully.");

        // Modify the modFilePath to save one folder lower
        string modFileName = $"{modId}.mod";
        string modFilePath = Path.Combine(Path.GetDirectoryName(outputFolderPath), modFileName);
        string modFolderPath3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Stellaris", "mod");
        string modContent = $"version=\"3.8.1\"\n" +
                            "tags={\n" +
                            "\t\"Spaceships\"\n" +
                            "\t\"Gameplay\"\n" +
                            "\t\"Technologies\"\n" +
                            "}\n" +
                            $"name=\"{GetModName(modId)}\"\n" +
                            "picture=\"thumbnail.png\"\n" +
                            "supported_version=\"3.8.*\"\n" +
                            $"archive=\"{Path.Combine(modFolderPath3.Replace('\\', '/'), modId, $"{modId}.zip").Replace('\\', '/')}\"\n" +
                            $"remote_file_id=\"{modId}\"";

        File.WriteAllText(modFilePath, modContent);

        Console.WriteLine(".mod file created successfully.");

        // Create a folder for the mod
        string modFolder = Path.Combine(Path.GetDirectoryName(outputFolderPath), modId);
        Directory.CreateDirectory(modFolder);

        // Move the zip file to the mod folder
        string modZipFilePath = Path.Combine(modFolder, $"{modId}.zip");
        File.Move(zipFilePath, modZipFilePath);

        Console.WriteLine("Zip file moved successfully.");

        string modFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Stellaris", "mod");

        // Move the mod file to the mod folder
        string modFilePathDestination = Path.Combine(modFolderPath, $"{modId}.mod");
        File.Move(modFilePath, modFilePathDestination);

        Console.WriteLine(".mod file moved to the mod folder.");

        // Create a folder for the mod
        string modFolder2 = Path.Combine(modFolderPath, modId);
        Directory.CreateDirectory(modFolder2);

        // Move the zip file to the mod folder
        string modZipFileDestination = Path.Combine(modFolder2, $"{modId}.zip");
        File.Move(modZipFilePath, modZipFileDestination);

        Console.WriteLine("Zip file moved to the mod folder.");

        // Exit the program
        Environment.Exit(0);


        Console.WriteLine("Press the Spacebar to continue...");
        while (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
        {
            // Wait for the user to press the Spacebar
        }

        Environment.Exit(0);
    }
    private static bool IsModIDValid(string modId)
    {
        string url = $"https://steamcommunity.com/sharedfiles/filedetails/?id={modId}";

        try
        {
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(url);

                // Check if the HTML contains the mod title
                return html.Contains("<div class=\"workshopItemTitle\">");
            }
        }
        catch (WebException ex)
        {
            // Handle any network or request errors
            Console.WriteLine("Error occurred while checking ModID validity: " + ex.Message);
            return false;
        }
    }


    private static string GetModName(string modId)
    {
        string url = $"https://steamcommunity.com/sharedfiles/filedetails/?id={modId}";

        try
        {
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(url);

                // Find the mod title within the HTML
                int startIndex = html.IndexOf("<div class=\"workshopItemTitle\">") + "<div class=\"workshopItemTitle\">".Length;
                int endIndex = html.IndexOf("</div>", startIndex);
                string modName = html.Substring(startIndex, endIndex - startIndex).Trim();

                return modName;
            }
        }
        catch (WebException ex)
        {
            // Handle any network or request errors
            Console.WriteLine("Error occurred while fetching the mod name: " + ex.Message);
            return string.Empty;
        }
    }


}
