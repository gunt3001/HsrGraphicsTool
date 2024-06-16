using System.ComponentModel;

namespace HsrGraphicsTool.Models;

/// <summary>
/// Enum representing the options for various graphics settings in Honkai: Star Rail.
/// Each setting have different sets of valid values.
/// </summary>
public enum GraphicsQualityOption
{
    [Description("Off")]
    Off = 0,
    
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