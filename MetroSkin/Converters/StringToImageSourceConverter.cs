namespace L4D2Toolbox.Themes.Converters;

public class StringToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var path = (string)value;
        if (string.IsNullOrWhiteSpace(path))
            return null;

        if (File.Exists(path))
        {
            var file = new FileInfo(path);
            var reader = new BinaryReader(File.Open(path, FileMode.Open));
            var bytes = reader.ReadBytes((int)file.Length);
            reader.Close();

            var bitmapImage = new BitmapImage
            {
                CacheOption = BitmapCacheOption.OnLoad
            };

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(bytes);
            bitmapImage.EndInit();

            return bitmapImage;
        }
        else
        {
            return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
