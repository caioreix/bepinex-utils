using System;
using System.IO;
using BepInEx.Logging;

namespace Utils.Logger;

public class Config {
    internal static ManualLogSource logger;
    private static string tempLogFile;


    // Setup the logs start configs.
    public static void Setup(ManualLogSource logger, string worldType = "") {
        Config.logger = logger;

        var suffix = "";
        if (worldType != "") {
            suffix = $"-{worldType}";
        }

        tempLogFile = $"{Settings.Config.PluginFolderPath}\\{Settings.Config.PluginGUID}{suffix}.txt";

        Log.Start($"Using \"{tempLogFile}\" to save logs.");
    }

    public static void TestSetup() { }

    internal static void logFile(object data, string level, string prefix = "") {
        if (Settings.ENV.LogOnTempFile) {
            using (StreamWriter w = File.AppendText(tempLogFile)) {
                var msg = $"{prefix}{DateTime.Now.ToString("hh:mm:ss")} [{level} {Settings.Config.PluginGUID}]: {data}";
                w.WriteLine(msg);
            }
        }
    }
}
