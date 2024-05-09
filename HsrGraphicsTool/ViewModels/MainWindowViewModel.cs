using System;
using System.Reactive;
using System.Reactive.Linq;
using HsrGraphicsTool.Models;
using HsrGraphicsTool.Models.Services;
using ReactiveUI;

namespace HsrGraphicsTool.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IHsrRegistryManager HsrRegistryManager { get; }

    /// <summary>
    /// Determines whether the controls are disabled because settings cannot be read
    /// </summary>
    public bool IsControlsDisabled { get; }

    /// <summary>
    /// Last known configuration read from the registry
    /// </summary>
    private HsrGraphicsConfiguration LastKnownConfig { get; set; }

    /// <summary>
    /// Flag set during change of preset to prevent reversion to Custom preset
    /// </summary>
    private bool IsSettingPreset { get; set; }

    /// <summary>
    /// Flag set during initial viewmodel creation to prevent reversion to Custom preset
    /// </summary>
    private bool IsInitialRun { get; set; } = true;

    // Backing fields
    private int _width;
    private int _height;
    private bool _isFullscreen;
    private GraphicsPresetOption _graphicsPreset;
    private int _fps;
    private bool _isVsyncEnabled;
    private decimal _renderingQuality;
    private GraphicsQualityOption _shadowQuality;
    private GraphicsQualityOption _reflectionQuality;
    private GraphicsQualityOption _characterQuality;
    private GraphicsQualityOption _environmentDetail;
    private GraphicsQualityOption _specialEffectsQuality;
    private GraphicsQualityOption _bloomEffect;
    private AntiAliasingMode _antiAliasing;
    private GraphicsQualityOption _lightQuality;
    private string _statusMessage;

    /// <summary>
    /// Resolution width
    /// </summary>
    public int Width
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    /// <summary>
    /// Resolution height
    /// </summary>
    public int Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    /// <summary>
    /// Fullscreen mode flag
    /// </summary>
    public bool IsFullscreen
    {
        get => _isFullscreen;
        set => this.RaiseAndSetIfChanged(ref _isFullscreen, value);
    }

    /// <summary>
    /// Graphics quality preset
    /// </summary>
    public GraphicsPresetOption GraphicsPreset
    {
        get => _graphicsPreset;
        set => this.RaiseAndSetIfChanged(ref _graphicsPreset, value);
    }

    /// <summary>
    /// FPS setting
    /// </summary>
    public int Fps
    {
        get => _fps;
        set => this.RaiseAndSetIfChanged(ref _fps, value);
    }

    /// <summary>
    /// V-Sync setting
    /// </summary>
    public bool IsVsyncEnabled
    {
        get => _isVsyncEnabled;
        set => this.RaiseAndSetIfChanged(ref _isVsyncEnabled, value);
    }

    /// <summary>
    /// Rendering quality setting
    /// </summary>
    public decimal RenderingQuality
    {
        get => _renderingQuality;
        set => this.RaiseAndSetIfChanged(ref _renderingQuality, value);
    }

    /// <summary>
    /// Shadow quality setting
    /// </summary>
    public GraphicsQualityOption ShadowQuality
    {
        get => _shadowQuality;
        set => this.RaiseAndSetIfChanged(ref _shadowQuality, value);
    }

    /// <summary>
    /// Reflection quality setting
    /// </summary>
    public GraphicsQualityOption ReflectionQuality
    {
        get => _reflectionQuality;
        set => this.RaiseAndSetIfChanged(ref _reflectionQuality, value);
    }

    /// <summary>
    /// Character quality setting
    /// </summary>
    public GraphicsQualityOption CharacterQuality
    {
        get => _characterQuality;
        set => this.RaiseAndSetIfChanged(ref _characterQuality, value);
    }

    /// <summary>
    /// Environment detail setting
    /// </summary>
    public GraphicsQualityOption EnvironmentDetail
    {
        get => _environmentDetail;
        set => this.RaiseAndSetIfChanged(ref _environmentDetail, value);
    }

    /// <summary>
    /// Special effects quality setting
    /// </summary>
    public GraphicsQualityOption SpecialEffectsQuality
    {
        get => _specialEffectsQuality;
        set => this.RaiseAndSetIfChanged(ref _specialEffectsQuality, value);
    }

    /// <summary>
    /// Bloom effect setting
    /// </summary>
    public GraphicsQualityOption BloomEffect
    {
        get => _bloomEffect;
        set => this.RaiseAndSetIfChanged(ref _bloomEffect, value);
    }

    /// <summary>
    /// Anti-aliasing setting
    /// </summary>
    public AntiAliasingMode AntiAliasing
    {
        get => _antiAliasing;
        set => this.RaiseAndSetIfChanged(ref _antiAliasing, value);
    }

    /// <summary>
    /// Light quality setting
    /// </summary>
    public GraphicsQualityOption LightQuality
    {
        get => _lightQuality;
        set => this.RaiseAndSetIfChanged(ref _lightQuality, value);
    }

    /// <summary>
    /// Status message to display to the user
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

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

        // Apply changes
        ApplyConfigToProperties(config);

        // Reactivity
        // Change advanced settings depending on preset chosen
        this.WhenAnyValue(x => x.GraphicsPreset)
            .Where(x => x != GraphicsPresetOption.Custom)
            .Subscribe(preset =>
            {
                var presetConfig = hsrRegistryManager.GetCustomGraphicsSettingsForPreset(preset);
                // Manually set a flag to prevent reversion to Custom. If somebody know a better way, enlighten me!
                IsSettingPreset = true;
                Fps = presetConfig.Fps;
                IsVsyncEnabled = presetConfig.IsVsyncEnabled;
                RenderingQuality = presetConfig.RenderScale;
                ShadowQuality = presetConfig.ShadowQuality;
                ReflectionQuality = presetConfig.ReflectionQuality;
                CharacterQuality = presetConfig.CharacterQuality;
                EnvironmentDetail = presetConfig.EnvironmentDetailQuality;
                SpecialEffectsQuality = presetConfig.SfxQuality;
                BloomEffect = presetConfig.BloomQuality;
                AntiAliasing = presetConfig.AntiAliasingMode;
                LightQuality = presetConfig.LightQuality;
                IsSettingPreset = false;
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
            .Where(x => !IsSettingPreset)
            .Subscribe(_ =>
            {
                // Another dirty workaround to prevent reversion to Custom preset during startup
                if (IsInitialRun)
                {
                    IsInitialRun = false;
                }
                else
                {
                    GraphicsPreset = GraphicsPresetOption.Custom;
                }
            });
        // Reverting changes
        DiscardChanges = ReactiveCommand.Create(() => ApplyConfigToProperties(LastKnownConfig));
        // Applying changes
        ApplyChanges = ReactiveCommand.Create(SaveChanges);
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
        
        StatusMessage = "Settings saved successfully.";
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
}