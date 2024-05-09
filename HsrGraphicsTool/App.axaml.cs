using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HsrGraphicsTool.Models.Services;
using HsrGraphicsTool.ViewModels;
using HsrGraphicsTool.Views;

namespace HsrGraphicsTool;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                // For now, we forego DI to keep it simple
                DataContext = new MainWindowViewModel(new HsrRegistryManager()),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}