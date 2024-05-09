using System.Text.Json.Serialization;

namespace HsrGraphicsTool.Models;

/// <summary>
/// Represents the graphics resolution settings in Honkai: Star Rail
/// </summary>
public record HsrResolution
{
    /// <summary>
    /// The resolution width
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// The resolution height
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// Whether resolution is full-screen
    /// </summary>
    [JsonPropertyName("isFullScreen")]
    public bool IsFullscreen { get; set; }
}