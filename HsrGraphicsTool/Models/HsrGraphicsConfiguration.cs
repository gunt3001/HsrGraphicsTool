namespace HsrGraphicsTool.Models;

/// <summary>
/// Record representing the graphics settings of Honkai: Star Rail
/// </summary>
public record HsrGraphicsConfiguration(GraphicsPresetOption GraphicsPreset,
    HsrResolution Resolution, HsrCustomGraphics CustomGraphics)
{
    /// <summary>
    /// The in-game "Graphics Quality" option
    /// </summary>
    public GraphicsPresetOption GraphicsPreset { get; } = GraphicsPreset;

    /// <summary>
    /// Advanced graphics settings when the GraphicsPreset is set to Custom.
    /// Known values from the game will be used for other presets from Very Low to Very High.
    /// </summary>
    public HsrCustomGraphics CustomGraphics { get; } = CustomGraphics;

    /// <summary>
    /// The graphics resolution settings
    /// </summary>
    public HsrResolution Resolution { get; } = Resolution;
}