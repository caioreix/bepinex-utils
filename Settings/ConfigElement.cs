using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace Utils.Settings;

public class ConfigElement<T>(ConfigEntry<T> original, string section, string key, T defaultValue, string description) {
    public string Section { get; } = section;
    public string Key { get; } = key;

    public string Description { get; } = description;

    public Type ElementType => typeof(T);

    public List<Action<T>> OnValueChanged = [];

    public readonly ConfigEntry<T> OriginalConfig = original;

    public T DefaultValue { get; } = defaultValue;

    public T Value {
        get => GetValue();
        set => SetValue(value);
    }

    public T ValueFromConfig() {
        Config.Reload();

        return Value;
    }

    private T GetValue() {
        return OriginalConfig.Value;
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
