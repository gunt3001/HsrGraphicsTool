using System.Text.Json.Serialization;

namespace HsrGraphicsTool.Models;

public record HsrCustomGraphics
{
    /// <summary>
    /// The in-game "FPS" option.
    /// Has 3 valid values, 30, 60, and 120. 120 FPS support is unofficial on PC.
    /// </summary>
    [JsonPropertyName("FPS")]
    public int Fps { get; set; }
    
    /// <summary>
    /// The in-game "V-Sync" option.
    /// </summary>
    [JsonPropertyName("EnableVSync")]
    public bool IsVsyncEnabled { get; set; }

    /// <summary>
    /// The in-game "Rendering Quality" option, representing the rendering scale.
    /// Only values between 0.6 to 2.0 in 0.2 interval (0.6, 0.8, 1.0, ...) are officially supported.
    /// </summary>
    [JsonPropertyName("RenderScale")]
    public decimal RenderScale { get; set; }
    
    /// <summary>
    /// A hidden, possibly unused setting that is always set to the last non-custom preset value used.
    /// </summary>
    [JsonPropertyName("ResolutionQuality")]
    public GraphicsPresetOption ResolutionQuality { get; set; }
    
    /// <summary>
    /// The in-game "Shadow Quality" option.
    /// Only values Off, Low, Medium, and High are officially supported.
    /// </summary>
    [JsonPropertyName("ShadowQuality")]
    public GraphicsQualityOption ShadowQuality { get; set; }
    
    /// <summary>
    /// The in-game "Light Quality" option.
    /// Only values VeryLow, Low, Medium, High, and VeryHigh are officially supported.
    /// </summary>
    [JsonPropertyName("LightQuality")]
    public GraphicsQualityOption LightQuality { get; set; }
    
    /// <summary>
    /// The in-game "Character Quality" option.
    /// Only values Low, Medium, and High are officially supported.
    /// </summary>
    [JsonPropertyName("CharacterQuality")]
    public GraphicsQualityOption CharacterQuality { get; set; }
    
    /// <summary>
    /// The in-game "Environment Detail" option.
    /// Only values VeryLow, Low, Medium, High, and VeryHigh are officially supported.
    /// </summary>
    [JsonPropertyName("EnvDetailQuality")]
    public GraphicsQualityOption EnvironmentDetailQuality { get; set; }
    
    /// <summary>
    /// The in-game "Reflection Quality" option.
    /// Only values VeryLow, Low, Medium, High, and VeryHigh are officially supported.
    /// </summary>
    [JsonPropertyName("ReflectionQuality")]
    public GraphicsQualityOption ReflectionQuality { get; set; }
    
    /// <summary>
    /// The in-game "Special Effects Quality" option.
    /// Only values VeryLow, Low, Medium, and High are officially supported.
    /// </summary>
    [JsonPropertyName("SFXQuality")]
    public GraphicsQualityOption SfxQuality { get; set; }
    
    /// <summary>
    /// The in-game "Bloom Effect" option.
    /// </summary>
    [JsonPropertyName("BloomQuality")]
    public GraphicsQualityOption BloomQuality { get; set; }
    
    /// <summary>
    /// The in-game "Anti-Aliasing" option.
    /// </summary>
    [JsonPropertyName("AAMode")]
    public AntiAliasingMode AntiAliasingMode { get; set; }
    
    /// <summary>
    /// Possibly used to enable MetalFX, Apple's upscaling technology.
    /// Most likely always set to false on PC.
    /// </summary>
    [JsonPropertyName("EnableMetalFXSU")]
    public bool IsMetalFxEnabled { get; set; }
}