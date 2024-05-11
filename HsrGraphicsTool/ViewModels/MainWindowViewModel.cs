using System;
using System.Reactive;
using System.Reactive.Linq;
using HsrGraphicsTool.Models;
using HsrGraphicsTool.Models.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace HsrGraphicsTool.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // Injected dependencies
    public IHsrRegistryManager HsrRegistryManager { get; }

    // Reactive properties

    // Regular properties

    /// <summary>
    /// Determines whether the controls are disabled because settings cannot be read
    /// </summary>
    public bool IsControlsDisabled { get; }

    /// <summary>
    /// Last known configuration read from the registry
    /// </summary>
    private HsrGraphicsConfiguration LastKnownConfig { get; set; }

    // Backing fields

    [Reactive] public int Width { get; set; }

    [Reactive] public int Height { get; set; }

    [Reactive] public bool IsFullscreen { get; set; }

    [Reactive] public GraphicsPresetOption GraphicsPreset { get; set; }

    [Reactive] public int Fps { get; set; }

    [Reactive] public bool IsVsyncEnabled { get; set; }

    [Reactive] public decimal RenderingQuality { get; set; }

    [Reactive] public GraphicsQualityOption ShadowQuality { get; set; }

    [Reactive] public GraphicsQualityOption ReflectionQuality { get; set; }

    [Reactive] public GraphicsQualityOption CharacterQuality { get; set; }

    [Reactive] public GraphicsQualityOption EnvironmentDetail { get; set; }

    [Reactive] public GraphicsQualityOption SpecialEffectsQuality { get; set; }

    [Reactive] public GraphicsQualityOption BloomEffect { get; set; }

    [Reactive] public AntiAliasingMode AntiAliasing { get; set; }

    [Reactive] public GraphicsQualityOption LightQuality { get; set; }

    [ObservableAsProperty] public string StatusMessage { get; }

    // Combo box options

    public GraphicsPresetOption[] GraphicsPresetOptions { get; } =
        Enum.GetValues<GraphicsPresetOption>();

    public int[] FpsOptions { get; } = { 30, 60, 120 };
    public decimal[] RenderingQualityOptions { get; } = { 0.6m, 0.8m, 1.0m, 1.2m, 1.4m, 1.6m, 1.8m, 2.0m };

    public GraphicsQualityOption[] ShadowQualityOptions { get; } =
    {
        GraphicsQualityOption.Off,
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
    };

    public GraphicsQualityOption[] ReflectionQualityOptions { get; } =
    {
        GraphicsQualityOption.VeryLow,
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
        GraphicsQualityOption.VeryHigh,
    };

    public GraphicsQualityOption[] CharacterQualityOptions { get; } =
    {
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
    };

    public GraphicsQualityOption[] EnvironmentDetailOptions { get; } =
    {
        GraphicsQualityOption.VeryLow,
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
        GraphicsQualityOption.VeryHigh,
    };

    public GraphicsQualityOption[] SpecialEffectsQualityOptions { get; } =
    {
        GraphicsQualityOption.VeryLow,
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
    };

    public GraphicsQualityOption[] BloomEffectOptions { get; } =
    {
        GraphicsQualityOption.Off,
        GraphicsQualityOption.VeryLow,
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
        GraphicsQualityOption.VeryHigh,
    };

    public AntiAliasingMode[] AntiAliasingOptions { get; } =
    {
        AntiAliasingMode.Off,
        AntiAliasingMode.Fxaa,
        AntiAliasingMode.Taa,
    };

    public GraphicsQualityOption[] LightQualityOptions { get; } =
    {
        GraphicsQualityOption.VeryLow,
        GraphicsQualityOption.Low,
        GraphicsQualityOption.Medium,
        GraphicsQualityOption.High,
        GraphicsQualityOption.VeryHigh,
    };

    // Commands

    public ReactiveCommand<Unit, Unit> DiscardChanges { get; }
    public ReactiveCommand<Unit, Unit> ApplyChanges { get; }

    public MainWindowViewModel(IHsrRegistryManager hsrRegistryManager)
    {
        HsrRegistryManager = hsrRegistryManager;
        // Load from registry
        var config = hsrRegistryManager.ReadConfig();
        if (config == null)
        {
            IsControlsDisabled = true;
            return;
        }

        // Set last known config to revert to if user discard changes
        LastKnownConfig = config;

        // Apply loaded config to UI components
        ApplyConfigToProperties(config);

        // Set up reactivity

        // Change graphics settings depending on preset chosen
        this.WhenAnyValue(x => x.GraphicsPreset)
            .Where(x => x != GraphicsPresetOption.Custom)
            .Select(hsrRegistryManager.GetCustomGraphicsSettingsForPreset)
            .Subscribe(settings =>
            {
                using (DelayChangeNotifications())
                {
                    Fps = settings.Fps;
                    IsVsyncEnabled = settings.IsVsyncEnabled;
                    RenderingQuality = settings.RenderScale;
                    ShadowQuality = settings.ShadowQuality;
                    ReflectionQuality = settings.ReflectionQuality;
                    CharacterQuality = settings.CharacterQuality;
                    EnvironmentDetail = settings.EnvironmentDetailQuality;
                    SpecialEffectsQuality = settings.SfxQuality;
                    BloomEffect = settings.BloomQuality;
                    AntiAliasing = settings.AntiAliasingMode;
                    LightQuality = settings.LightQuality;
                }
            });

        // Reset preset to Custom if any of its dependencies change
        this.WhenAnyValue(
                x => x.Fps,
                x => x.IsVsyncEnabled,
                x => x.RenderingQuality,
                x => x.ShadowQuality,
                x => x.ReflectionQuality,
                x => x.CharacterQuality,
                x => x.EnvironmentDetail,
                x => x.SpecialEffectsQuality,
                x => x.BloomEffect,
                x => x.AntiAliasing,
                x => x.LightQuality,
                (_, _, _, _, _, _, _, _, _, _, _) => Unit.Default)
            .Where(_ => GraphicsPreset != GraphicsPresetOption.Custom)
            .Where(_ => !IsPresetValid())
            .Subscribe(_ => { GraphicsPreset = GraphicsPresetOption.Custom; });

        // Button commands
        // Reverting changes
        DiscardChanges = ReactiveCommand.Create(() => ApplyConfigToProperties(LastKnownConfig));
        // Applying changes
        ApplyChanges = ReactiveCommand.Create(SaveChanges);
        // Display success message
        ApplyChanges
            .Select(_ => "Settings saved successfully.")
            .ToPropertyEx(this, x => x.StatusMessage);
    }

    private void SaveChanges()
    {
        var newConfig = new HsrGraphicsConfiguration
        {
            Resolution = new HsrResolution
            {
                Width = Width,
                Height = Height,
                IsFullscreen = IsFullscreen,
            },
            GraphicsPreset = GraphicsPreset,
            CustomGraphics = new HsrCustomGraphics
            {
                Fps = Fps,
                IsVsyncEnabled = IsVsyncEnabled,
                RenderScale = RenderingQuality,
                ShadowQuality = ShadowQuality,
                ReflectionQuality = ReflectionQuality,
                CharacterQuality = CharacterQuality,
                EnvironmentDetailQuality = EnvironmentDetail,
                SfxQuality = SpecialEffectsQuality,
                BloomQuality = BloomEffect,
                AntiAliasingMode = AntiAliasing,
                LightQuality = LightQuality,
            }
        };
        HsrRegistryManager.SaveConfig(newConfig);
        // Save as new last known config
        LastKnownConfig = newConfig;
    }

    /// <summary>
    /// Applies the configuration to the view model properties
    /// </summary>
    /// <param name="config">Configuration to use</param>
    private void ApplyConfigToProperties(HsrGraphicsConfiguration config)
    {
        Width = config.Resolution.Width;
        Height = config.Resolution.Height;
        IsFullscreen = config.Resolution.IsFullscreen;
        GraphicsPreset = config.GraphicsPreset;
        Fps = config.CustomGraphics.Fps;
        IsVsyncEnabled = config.CustomGraphics.IsVsyncEnabled;
        RenderingQuality = config.CustomGraphics.RenderScale;
        ShadowQuality = config.CustomGraphics.ShadowQuality;
        ReflectionQuality = config.CustomGraphics.ReflectionQuality;
        CharacterQuality = config.CustomGraphics.CharacterQuality;
        EnvironmentDetail = config.CustomGraphics.EnvironmentDetailQuality;
        SpecialEffectsQuality = config.CustomGraphics.SfxQuality;
        BloomEffect = config.CustomGraphics.BloomQuality;
        AntiAliasing = config.CustomGraphics.AntiAliasingMode;
        LightQuality = config.CustomGraphics.LightQuality;
    }

    /// <summary>
    /// Check if the current preset values are match the selected preset
    /// </summary>
    /// <returns></returns>
    private bool IsPresetValid()
    {
        // Custom preset is always valid
        if (GraphicsPreset == GraphicsPresetOption.Custom) return true;

        var preset = HsrRegistryManager.GetCustomGraphicsSettingsForPreset(GraphicsPreset);

        // Compare the preset values
        // Note that two hidden values are ignored, the ResolutionQuality and MetalFx flag
        return Fps == preset.Fps &&
               IsVsyncEnabled == preset.IsVsyncEnabled &&
               RenderingQuality == preset.RenderScale &&
               ShadowQuality == preset.ShadowQuality &&
               ReflectionQuality == preset.ReflectionQuality &&
               CharacterQuality == preset.CharacterQuality &&
               EnvironmentDetail == preset.EnvironmentDetailQuality &&
               SpecialEffectsQuality == preset.SfxQuality &&
               BloomEffect == preset.BloomQuality &&
               AntiAliasing == preset.AntiAliasingMode &&
               LightQuality == preset.LightQuality;
    }
}