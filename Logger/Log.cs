using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Utils.Logger;

public class Log {
    private static ConcurrentDictionary<string, long> timedLog = new ConcurrentDictionary<string, long>();
    private static List<string> firstLog = new List<string>();

    // Info logs
    public static void Info(object data) {
        Config.logger.LogInfo(data);
        Config.logFile(data, "Info:   ");
    }

    // Error logs
    public static void Error(object data) {
        Config.logger.LogError(data);
        Config.logFile(data, "Error:  ");
    }

    // Debug logs
    public static void Debug(object data) {
        Config.logger.LogDebug(data);
        Config.logFile(data, "Debug:  ");
    }

    // Fatal logs
    public static void Fatal(object data) {
        Config.logger.LogFatal(data);
        Config.logFile(data, "Fatal:  ");
    }

    // Warning logs
    public static void Warning(object data) {
        Config.logger.LogWarning(data);
        Config.logFile(data, "Warning:");
    }

    // Message logs
    public static void Message(object data) {
        Config.logger.LogMessage(data);
        Config.logFile(data, "Message:");
    }

    // Start logs
    public static void Start(object data) {
        Config.logger.LogMessage(data);
        Config.logFile(data, "Start:  ", "\n");
    }

    // Trace logs
    public static void Trace(object data) {
        if (Settings.ENV.EnableTraceLogs) {
            Config.logger.LogDebug(data);
            Config.logFile(data, "Trace:  ");
        }
    }

    // Struct logs
    public static void Struct<T>(T data) {
        if (Settings.ENV.EnableTraceLogs) {
            var msg = structToString(data);
            Config.logger.LogDebug(msg);
            Config.logFile(msg, "Struct: ");
        }
    }

    //Timed logs by id
    public static void Timed(Action action, int ms, string id = "") {
        if (blocked(ms, id)) return;
        action();
    }

    // First logs by id
    public static void First(Action action, string id = "") {
        if (!first(id)) return;
        action();
    }

    // Helpers

    private static string structToString<T>(T data) {
        var type = data.GetType();
        var fields = type.GetFields();
        var properties = type.GetProperties();

        var values = new Dictionary<string, object>();
        Array.ForEach(properties, (property) => {
            values.TryAdd(property.Name, property.GetValue(data));
        });
        Array.ForEach(fields, (field) => {
            values.TryAdd(field.Name, field.GetValue(data));
        });
        var lines = new List<string>();
        foreach (var value in values) {
            lines.Add($"\"{value.Key}\":\"{value.Value}\"");
        }

        return $"\"{type.ToString()}\": " + "{" + String.Join(",", lines) + "}";
    }

    private static bool first(string id) {
        if (firstLog.Contains(id)) return false;
        firstLog.Add(id);
        return true;
    }

    private static bool blocked(int ms, string id) {
        var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (!timedLog.TryGetValue(id, out var timestamp)) {
            var newTimestamp = DateTimeOffset.Now.AddMilliseconds(ms).ToUnixTimeMilliseconds();
            timedLog[id] = newTimestamp;
            return true;
        }

        if (currentTimestamp < timestamp) {
            return true;
        }

        timedLog.TryRemove(id, out _);
        return false;
    }
}
