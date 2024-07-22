using System.Drawing;
using System.Runtime.InteropServices;

namespace HsrGraphicsTool.Models;

/// <summary>
/// Utilities for capturing Windows display information using the Windows GDI API.
/// Adapted from this S/O answer: https://stackoverflow.com/a/76670176
/// </summary>
public static class DisplayUtils
{
    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(nint hdc, int nIndex);

    private enum DeviceCap
    {
        Desktopvertres = 117,
        Desktophorzres = 118
    }

    public static Size GetPhysicalDisplaySize()
    {
        var g = Graphics.FromHwnd(nint.Zero);
        var desktop = g.GetHdc();

        var physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.Desktopvertres);
        var physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.Desktophorzres);

        return new Size(physicalScreenWidth, physicalScreenHeight);
    } 
}