using System.Globalization;

namespace DevNote.Converters;

public class RecordButtonColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRecording)
            return isRecording ? Color.FromArgb("#E53935") : Color.FromArgb("#304040");
        return Color.FromArgb("#304040");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
