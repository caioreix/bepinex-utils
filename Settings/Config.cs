using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Configuration;

namespace Utils.Settings;

public class Config {
    public static string PluginGUID;
    public static string PluginFolderPath;

    internal static ConfigFile cfg;
    private static List<Action> configActions = new List<Action>();

    public static ConfigElement<T> Bind<T>(string section, string key, T defaultValue, string description) {
        var config = cfg.Bind(
                section,
                key,
                defaultValue,
                description
            );

        return new(config, section, key, defaultValue, description);
    }

    public static void Save() {
        cfg.Save();
    }

    public static void Setup(string pluginGUID, ConfigFile config, int skipCaller = 4, params Action[] actions) {
        PluginGUID = pluginGUID;
        cfg = config;

        PluginFolderPath = $"{BepInEx.Paths.ConfigPath}\\{PluginGUID}";
        if (!Directory.Exists(PluginFolderPath)) {
            Directory.CreateDirectory(PluginFolderPath);
        }

        ENV.Debug.Setup(skipCaller);
        AddConfigActions(actions);
    }

    public static void Load() {
        if (configActions == null || configActions.Count == 0) return;
        foreach (var action in configActions) {
            action();
        }
    }

    public static void AddConfigActions(params Action[] actions) {
        configActions.AddRange(actions);
    }
}
