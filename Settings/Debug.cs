using System.Reflection;

namespace Utils.Settings;

public static class ENV {
    public static bool LogOnTempFile = false;
    public static bool EnableTraceLogs = false;

    public class Debug {
        private static string debugSection = "🪲Debug";
        private static ConfigElement<bool> DebugLogOnTempFile;
        private static ConfigElement<bool> DebugEnableTraceLogs;

        public static void Setup(int skipCaller) {
            Config.AddConfigActions(
                () => load(skipCaller)
            );
        }

        // Load the plugin debug variables.
        private static void load(int skipCaller) {
            if (enableDebugConfigs(skipCaller)) {
                DebugLogOnTempFile = Config.Bind(
                    debugSection,
                    "LogOnTempFile",
                    false,
                    "Enabled, will log every plugin log on a temp file"
                );

                DebugEnableTraceLogs = Config.Bind(
                    debugSection,
                    "EnableTraceLogs",
                    false,
                    "Enabled, will print Trace logs (Debug output in BepInEx)"
                );
            }

            validateValues();
        }

        private static void validateValues() {
            if (DebugLogOnTempFile != null) {
                LogOnTempFile = DebugLogOnTempFile.Value;
            }

            if (DebugEnableTraceLogs != null) {
                EnableTraceLogs = DebugEnableTraceLogs.Value;
            }

            Config.cfg.Save();
        }

        private static bool enableDebugConfigs(int skipCaller) {
            var assemblyConfigurationAttribute = new System.Diagnostics.StackTrace().GetFrame(skipCaller).GetMethod().DeclaringType.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;
            return buildConfigurationName != "Release";
        }
    }
}
