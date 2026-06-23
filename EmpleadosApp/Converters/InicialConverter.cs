using System.Globalization;

namespace EmpleadosApp.Converters;

public class InicialConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var texto = value as string;
        return string.IsNullOrWhiteSpace(texto)
            ? "?"
            : texto.Trim()[0].ToString().ToUpperInvariant();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
