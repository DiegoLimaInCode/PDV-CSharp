namespace PDVCSharp.WPF.Navigation;

public interface INavigationService
{
    void Navigate(AppScreen screen);
    T? GetScreen<T>() where T : class;
}
