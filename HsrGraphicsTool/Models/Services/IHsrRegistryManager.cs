namespace HsrGraphicsTool.Models.Services;

/// <summary>
/// Interface for reading and writing Honkai: Star Rail settings via its registry keys.
/// </summary>
public interface IHsrRegistryManager
{
    /// <summary>
    /// Reads the current Honkai: Star Rail graphics settings from the registry.
    /// </summary>
    /// <returns>Current graphics settings, or null if no valid configuration found.</returns>
    HsrGraphicsConfiguration? ReadConfig();

    /// <summary>
    /// Save the given Honkai: Star Rail grapics settings to the registry
    /// </summary>
    /// <param name="config">Graphics settings to save</param>
    void SaveConfig(HsrGraphicsConfiguration config);

    /// <summary>
    /// Gets the advanced graphics settings for a given preset.
    /// </summary>
    /// <param name="preset">Graphics preset option</param>
    /// <returns>Custom graphics settings object</returns>
    HsrCustomGraphics GetCustomGraphicsSettingsForPreset(GraphicsPresetOption preset);
}