using System;
using System.Collections.Concurrent;

namespace Utils.Database;

public static class Cache {
    public static ConcurrentDictionary<string, long> LastUpdate = new ConcurrentDictionary<string, long>();
    public static ConcurrentDictionary<string, bool> Cached = new ConcurrentDictionary<string, bool>();

    public static bool IsBlocked(string key, long blockedDuration) {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (LastUpdate.TryGetValue(key, out long timestamp) && (now - timestamp) < blockedDuration) {
            return true;
        }

        LastUpdate.AddOrUpdate(key, now, (_, _) => now);
        return false;
    }

    public static bool Key(string key, bool cache = true) {
        if (!cache) { return false; }
        return Cached.AddOrUpdate(key, true, (_, _) => true);
    }

    public static bool Exists(string key) {
        if (Cached.TryGetValue(key, out bool called) && called) {
            return true;
        }
        return false;
    }

    public static bool RemoveKey(string key) {
        return Cached.TryRemove(key, out bool _);
    }

    public static void Clear() {
        Cached.Clear();
        LastUpdate.Clear();
    }
}
