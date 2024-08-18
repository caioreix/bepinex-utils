using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace Utils.Settings;

public class ConfigElement<T> {
    public string Section { get; }
    public string Key { get; }

    public string Description { get; }

    public Type ElementType => typeof(T);

    public List<Action<T>> OnValueChanged = new List<Action<T>>();

    private ConfigEntry<T> OriginalConfig;

    public T Value {
        get => GetValue();
        set => SetValue(value);
    }

    public T DefaultValue { get; }


    private T GetValue() {
        return OriginalConfig.Value;
    }

    public ConfigElement(ConfigEntry<T> original, string section, string key, T defaultValue, string description) {
        OriginalConfig = original;
        Section = section;
        Key = key;
        DefaultValue = defaultValue;
        Description = description;
    }

    private void SetValue(T value) {
        if ((Value == null && value == null) || (Value != null && Value.Equals(value)))
            return;

        OriginalConfig.Value = value;

        foreach (var action in OnValueChanged) {
            action?.Invoke(value);
        }
    }
}
