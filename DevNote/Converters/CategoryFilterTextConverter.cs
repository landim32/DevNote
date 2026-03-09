using System.Globalization;

namespace DevNote.Converters;

public class CategoryFilterTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text && !string.IsNullOrEmpty(text))
            return text == "__archived__" ? "Arquivadas" : text;
        return "Todas";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
