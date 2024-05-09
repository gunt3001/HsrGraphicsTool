using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Win32;

namespace HsrGraphicsTool.Models.Services;

/// <inheritdoc />
/// <summary>
/// Class for reading and writing Honkai: Star Rail settings via its registry keys.
/// </summary>
class HsrRegistryManager : IHsrRegistryManager
{
    /// <summary>
    /// Path to the Honkai: Star Rail registry key from CurrentUser root.
    /// </summary>
    private const string RegistryPath = @"Software\Cognosphere\Star Rail";

    /// <summary>
    /// Prefix for the registry value name of the graphics preset setting.
    /// </summary>
    private const string GraphicsPresetValueNamePrefix = "GraphicsSettings_GraphicsQuality_";

    /// <summary>
    /// Prefix for the registry value name of the resolution setting.
    /// </summary>
    private const string GraphicsResolutionValueNamePrefix = "GraphicsSettings_PCResolution_";

    /// <summary>
    /// Prefix for the registry value name of the custom graphics settings.
    /// </summary>
    private const string GraphicsCustomSettingsValueNamePrefix = "GraphicsSettings_Model_";

    /// <summary>
    /// Suffix for the registry value name of the custom graphics settings, used to create one when none exists.
    /// </summary>
    private const string GraphicsCustomSettingsValueNameSuffix = "h2986158309";

    /// <inheritdoc />
    public HsrGraphicsConfiguration? ReadConfig()
    {
        // Access the game's registry root
        using var hsrSettings = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
        if (hsrSettings == null) return null;

        // Read the graphics preset value
        // Because the game appends the game's version to the name, we just find the first one that matches the prefix
        var hsrPresetName = hsrSettings.GetValueNames()
            .FirstOrDefault(x => x.StartsWith(GraphicsPresetValueNamePrefix));
        if (hsrPresetName == null) return null;
        var presetRawValue = (int)hsrSettings.GetValue(hsrPresetName, -1);
        if (presetRawValue == -1) return null;
        var preset = (GraphicsPresetOption)presetRawValue;

        // Read the resolution settings
        var hsrResolutionName = hsrSettings.GetValueNames()
            .FirstOrDefault(x => x.StartsWith(GraphicsResolutionValueNamePrefix));
        if (hsrResolutionName == null) return null;
        var resolution = ReadRegistryJson<HsrResolution>(hsrSettings, hsrResolutionName);
        if (resolution == null) return null;

        // If the graphics preset is set to Custom, read the custom graphics settings
        HsrCustomGraphics? customGraphics = null;
        var hsrCustomSettingsName = hsrSettings.GetValueNames()
            .FirstOrDefault(x => x.StartsWith(GraphicsCustomSettingsValueNamePrefix));
        if (preset == GraphicsPresetOption.Custom && hsrCustomSettingsName != null)
        {
            customGraphics = ReadRegistryJson<HsrCustomGraphics>(hsrSettings, hsrCustomSettingsName);
        }

        // If graphics object is still empty, read default values for the preset
        customGraphics ??= GetCustomGraphicsSettingsForPreset(preset);
        return new HsrGraphicsConfiguration
        {
            GraphicsPreset = preset,
            Resolution = resolution,
            CustomGraphics = customGraphics,
        };
    }

    /// <inheritdoc />
    public void SaveConfig(HsrGraphicsConfiguration config)
    {
        // Access the game's registry root
        using var hsrSettings = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
        if (hsrSettings == null) throw new InvalidOperationException("Could not access the game's registry key.");

        // Save the graphics preset value
        var hsrPresetName = hsrSettings.GetValueNames()
            .FirstOrDefault(x => x.StartsWith(GraphicsPresetValueNamePrefix));
        if (hsrPresetName == null) throw new InvalidOperationException("Could not find the graphics preset value.");
        hsrSettings.SetValue(hsrPresetName, (int)config.GraphicsPreset);

        // Save the resolution settings
        var hsrResolutionName = hsrSettings.GetValueNames()
            .FirstOrDefault(x => x.StartsWith(GraphicsResolutionValueNamePrefix));
        if (hsrResolutionName == null) throw new InvalidOperationException("Could not find the resolution value.");
        SaveRegistryJson(hsrSettings, hsrResolutionName, config.Resolution);

        // If the graphics preset is set to Custom, save the custom graphics settings
        if (config.GraphicsPreset != GraphicsPresetOption.Custom) return;

        var hsrCustomSettingsName = hsrSettings.GetValueNames()
                                        .FirstOrDefault(x => x.StartsWith(GraphicsCustomSettingsValueNamePrefix)) ??
                                    GraphicsCustomSettingsValueNamePrefix + GraphicsCustomSettingsValueNameSuffix;
        // Override a few hidden values
        config.CustomGraphics.ResolutionQuality = config.GraphicsPreset;
        config.CustomGraphics.IsMetalFxEnabled = false;
        SaveRegistryJson(hsrSettings, hsrCustomSettingsName, config.CustomGraphics);
    }

    /// <inheritdoc />
    public HsrCustomGraphics GetCustomGraphicsSettingsForPreset(GraphicsPresetOption preset)
    {
        return preset switch
        {
            GraphicsPresetOption.Custom => new HsrCustomGraphics()
            {
                // Technically custom preset doesn't have a valid setting, but we'll use the lowest settings for now
                Fps = 60,
                IsVsyncEnabled = false,
                RenderScale = 0.8m,
                ResolutionQuality = GraphicsPresetOption.VeryLow,
                ShadowQuality = GraphicsQualityOption.Low,
                LightQuality = GraphicsQualityOption.VeryLow,
                CharacterQuality = GraphicsQualityOption.Low,
                EnvironmentDetailQuality = GraphicsQualityOption.VeryLow,
                ReflectionQuality = GraphicsQualityOption.VeryLow,
                SfxQuality = GraphicsQualityOption.VeryLow,
                BloomQuality = GraphicsQualityOption.VeryLow,
                AntiAliasingMode = AntiAliasingMode.Taa,
                IsMetalFxEnabled = false,
            },
            GraphicsPresetOption.VeryLow => new HsrCustomGraphics()
            {
                Fps = 60,
                IsVsyncEnabled = false,
                RenderScale = 0.8m,
                ResolutionQuality = GraphicsPresetOption.VeryLow,
                ShadowQuality = GraphicsQualityOption.Low,
                LightQuality = GraphicsQualityOption.VeryLow,
                CharacterQuality = GraphicsQualityOption.Low,
                EnvironmentDetailQuality = GraphicsQualityOption.VeryLow,
                ReflectionQuality = GraphicsQualityOption.VeryLow,
                SfxQuality = GraphicsQualityOption.VeryLow,
                BloomQuality = GraphicsQualityOption.VeryLow,
                AntiAliasingMode = AntiAliasingMode.Taa,
                IsMetalFxEnabled = false,
            },
            GraphicsPresetOption.Low => new HsrCustomGraphics()
            {
                Fps = 60,
                IsVsyncEnabled = true,
                RenderScale = 1.0m,
                ResolutionQuality = GraphicsPresetOption.Low,
                ShadowQuality = GraphicsQualityOption.Low,
                LightQuality = GraphicsQualityOption.Low,
                CharacterQuality = GraphicsQualityOption.Low,
                EnvironmentDetailQuality = GraphicsQualityOption.Low,
                ReflectionQuality = GraphicsQualityOption.Low,
                SfxQuality = GraphicsQualityOption.Low,
                BloomQuality = GraphicsQualityOption.Low,
                AntiAliasingMode = AntiAliasingMode.Taa,
                IsMetalFxEnabled = false,
            },
            GraphicsPresetOption.Medium => new HsrCustomGraphics()
            {
                Fps = 60,
                IsVsyncEnabled = true,
                RenderScale = 1.0m,
                ResolutionQuality = GraphicsPresetOption.Medium,
                ShadowQuality = GraphicsQualityOption.Medium,
                LightQuality = GraphicsQualityOption.Medium,
                CharacterQuality = GraphicsQualityOption.Medium,
                EnvironmentDetailQuality = GraphicsQualityOption.Medium,
                ReflectionQuality = GraphicsQualityOption.Medium,
                SfxQuality = GraphicsQualityOption.Medium,
                BloomQuality = GraphicsQualityOption.Medium,
                AntiAliasingMode = AntiAliasingMode.Taa,
                IsMetalFxEnabled = false,
            },
            GraphicsPresetOption.High => new HsrCustomGraphics()
            {
                Fps = 60,
                IsVsyncEnabled = true,
                RenderScale = 1.2m,
                ResolutionQuality = GraphicsPresetOption.High,
                ShadowQuality = GraphicsQualityOption.High,
                LightQuality = GraphicsQualityOption.High,
                CharacterQuality = GraphicsQualityOption.High,
                EnvironmentDetailQuality = GraphicsQualityOption.High,
                ReflectionQuality = GraphicsQualityOption.High,
                SfxQuality = GraphicsQualityOption.High,
                BloomQuality = GraphicsQualityOption.High,
                AntiAliasingMode = AntiAliasingMode.Taa,
                IsMetalFxEnabled = false,
            },
            GraphicsPresetOption.VeryHigh => new HsrCustomGraphics()
            {
                Fps = 60,
                IsVsyncEnabled = true,
                RenderScale = 1.4m,
                ResolutionQuality = GraphicsPresetOption.VeryHigh,
                ShadowQuality = GraphicsQualityOption.High,
                LightQuality = GraphicsQualityOption.VeryHigh,
                CharacterQuality = GraphicsQualityOption.High,
                EnvironmentDetailQuality = GraphicsQualityOption.VeryHigh,
                ReflectionQuality = GraphicsQualityOption.VeryHigh,
                SfxQuality = GraphicsQualityOption.High,
                BloomQuality = GraphicsQualityOption.VeryHigh,
                AntiAliasingMode = AntiAliasingMode.Taa,
                IsMetalFxEnabled = false,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, null)
        };
    }

    /// <summary>
    /// Reads a JSON-serialized object from the registry.
    /// </summary>
    /// <param name="key">Registry key to read from.</param>
    /// <param name="valueName">Registry value name within the key to read from.</param>
    /// <typeparam name="T">Type to deserialize the value into.</typeparam>
    /// <returns>Deserialized object from the read registry value</returns>
    private T? ReadRegistryJson<T>(RegistryKey key, string valueName) where T : class
    {
        // Read the JSON string from the registry
        var bytes = key.GetValue(valueName, null);
        // Parse the bytes into a string
        var json = bytes == null
            ? null
            : System.Text.Encoding.UTF8.GetString((byte[])bytes);
        // Also remove the null terminator
        json = json?.TrimEnd('\0');
        return json == null
            ? null
            : JsonSerializer.Deserialize<T>(json);
    }

    private void SaveRegistryJson(RegistryKey key, string valueName, object value)
    {
        // Serialize object to JSON
        var json = JsonSerializer.Serialize(value);
        // Create byte array of JSON string (and null-terminate it)
        var bytes = System.Text.Encoding.UTF8.GetBytes(json + '\0');
        // Write the byte array to the registry
        key.SetValue(valueName, bytes);
    }
}