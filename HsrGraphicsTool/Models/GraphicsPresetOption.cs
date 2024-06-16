using System.ComponentModel;

namespace HsrGraphicsTool.Models;

/// <summary>
/// Enum representing the presets for the in-game "Graphics Quality" setting
/// </summary>
public enum GraphicsPresetOption
{
    [Description("Custom")]
    Custom = 0,
    
    [Description("Very Low")]
    VeryLow = 1,
    
    [Description("Low")]
    Low = 2,
    
    [Description("Medium")]
    Medium = 3,
    
    [Description("High")]
    High = 4,
    
    [Description("Very High")]
    VeryHigh = 5,
}