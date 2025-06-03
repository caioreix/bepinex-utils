using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using Utils.Logger;

namespace Utils.Injection;

public static class IL2CPP {
    public static void Unregister<T>() where T : class {
        try {
            if (ClassInjector.IsTypeRegisteredInIl2Cpp<T>()) {
                {
                    var field = typeof(ClassInjector).GetField("InjectedTypes", BindingFlags.NonPublic | BindingFlags.Static);
                    var value = field.GetValue(null);
                    var InjectedTypes = value as HashSet<string>;
                    InjectedTypes?.Remove(typeof(T).FullName);
                }

                {
                    var helpersType = typeof(ClassInjector).Assembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers");
                    var field = helpersType.GetField("s_ClassNameLookup", BindingFlags.NonPublic | BindingFlags.Static);
                    var value = field.GetValue(null);
                    if (value is Dictionary<(string _namespace, string _class, IntPtr imagePtr), IntPtr> s_ClassNameLookup) {
                        var type = typeof(T);
                        string className = type.Name;
                        string namespaceName = type.Namespace ?? string.Empty;

                        s_ClassNameLookup.Keys.Where(key => key._class == className && key._namespace == namespaceName).ToList().ForEach(key => {
                            s_ClassNameLookup.Remove(key);
                        });
                    }
                }
            }
        } catch (Exception e) {
            Log.Warning($"Failed to unregister behavior {typeof(T).FullName}: {e.Message}");
        }
    }
}
