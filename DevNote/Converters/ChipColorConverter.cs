using System.Globalization;

namespace DevNote.Converters;

public class ChipColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var current = values[0] as string ?? "";
        var selected = values[1] as string ?? "";
        bool isSelected = current == selected;
        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var param = parameter as string ?? "";

        if (param == "bg")
        {
            if (isSelected)
                return Color.FromArgb(isDark ? "#3A6050" : "#304040");
            return Color.FromArgb(isDark ? "#3D2E10" : "#D0E0E0");
        }

        // text
        if (isSelected)
            return Color.FromArgb(isDark ? "#FFFFFF" : "#FFFFFF");
        return Color.FromArgb(isDark ? "#FFB74D" : "#304040");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
