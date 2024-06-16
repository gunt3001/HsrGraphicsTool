using System.ComponentModel;

namespace HsrGraphicsTool.Models;

/// <summary>
/// Enum representing the options for the in-game "Anti-Aliasing" setting
/// </summary>
public enum AntiAliasingMode
{
    [Description("Off")]
    Off,
    
    [Description("TAA")]
    Taa = 1,
    
    [Description("FXAA")]
    Fxaa = 2,
}