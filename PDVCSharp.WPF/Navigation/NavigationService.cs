using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PDVCSharp.WPF.Navigation;

public sealed class NavigationService : INavigationService
{
    private readonly ContentControl _host;
    private readonly Dictionary<AppScreen, UserControl> _screens;

    public NavigationService(ContentControl host, Dictionary<AppScreen, UserControl> screens)
    {
        _host = host;
        _screens = screens;
    }

    public void Navigate(AppScreen screen)
    {
        if (!_screens.TryGetValue(screen, out var control))
        {
            return;
        }

        if (_host.Content is null)
        {
            Show(control);
            return;
        }

        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(100))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };
        fadeOut.Completed += (_, _) => Show(control);
        _host.BeginAnimation(UIElement.OpacityProperty, fadeOut);
    }

    private void Show(UserControl control)
    {
        _host.Content = control;
        if (control is IScreenActivation activation)
        {
            activation.OnNavigatedTo();
        }

        _host.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(160))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        });
    }

    public T? GetScreen<T>() where T : class
        => _screens.Values.OfType<T>().FirstOrDefault();
}
