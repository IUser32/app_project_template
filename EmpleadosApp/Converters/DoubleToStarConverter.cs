using System.Globalization;

namespace EmpleadosApp.Converters;

public class DoubleToStarConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var peso = value is double d ? d : 0;
        return new GridLength(peso <= 0 ? 0.0001 : peso, GridUnitType.Star);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
