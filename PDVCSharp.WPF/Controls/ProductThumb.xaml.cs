using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PDVCSharp.WPF.Controls;

public partial class ProductThumb : UserControl
{
    public static readonly DependencyProperty ImagePathProperty =
        DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(ProductThumb), new PropertyMetadata(null, OnVisualChanged));

    public static readonly DependencyProperty ProductNameProperty =
        DependencyProperty.Register(nameof(ProductName), typeof(string), typeof(ProductThumb), new PropertyMetadata(string.Empty, OnVisualChanged));

    public ProductThumb()
    {
        InitializeComponent();
        Loaded += (_, _) => Refresh();
    }

    public string? ImagePath
    {
        get => (string?)GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public string ProductName
    {
        get => (string)GetValue(ProductNameProperty);
        set => SetValue(ProductNameProperty, value);
    }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProductThumb thumb)
        {
            thumb.Refresh();
        }
    }

    private void Refresh()
    {
        if (LetterText is null || Photo is null)
        {
            return;
        }

        var name = ProductName?.Trim();
        LetterText.Text = string.IsNullOrWhiteSpace(name) ? "?" : name[0].ToString().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(ImagePath))
        {
            Photo.Source = null;
            Photo.Visibility = Visibility.Collapsed;
            return;
        }

        try
        {
            Photo.Source = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
            Photo.Visibility = Visibility.Visible;
        }
        catch
        {
            Photo.Source = null;
            Photo.Visibility = Visibility.Collapsed;
        }
    }
}
