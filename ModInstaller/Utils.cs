using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Data;

namespace ModInstaller;

public static class Utils
{

    private static readonly string[] sizeSuffixes =
        { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string GetSizeSuffix(long value, int decimalPlaces = 2)
    {
        if (value < 0) { return "-" + GetSizeSuffix(-value); }
        if (value == 0) { return "0.0 bytes"; }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            sizeSuffixes[mag]);
    }

    static public string CalculateMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
internal class ReverseBooleanToVisibilityConverter : IValueConverter
{
    #region Methods
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) return Visibility.Hidden;

        return (bool)value ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new Exception("Can not ConvertBack");
    }
    #endregion
}

internal class BooleanToVisibilityConverter : IValueConverter
{
    #region Methods
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) return Visibility.Hidden;

        return (bool)value ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new Exception("Can not ConvertBack");
    }
    #endregion
}