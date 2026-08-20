using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Controls;

public partial class AppHeader : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(AppHeader), new PropertyMetadata(string.Empty));

    public AppHeader()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
