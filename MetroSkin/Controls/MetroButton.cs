namespace MetroSkin.Controls;

public class MetroButton : Button
{
    /// <summary>
    /// 按钮图标
    /// </summary>
    public string Icon
    {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(string), typeof(MetroButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 按钮图标前景色
    /// </summary>
    public Brush IconForeground
    {
        get { return (Brush)GetValue(IconForegroundProperty); }
        set { SetValue(IconForegroundProperty, value); }
    }
    public static readonly DependencyProperty IconForegroundProperty =
        DependencyProperty.Register("IconForeground", typeof(Brush), typeof(MetroButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));
}
