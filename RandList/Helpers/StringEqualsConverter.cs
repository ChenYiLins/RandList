using Microsoft.UI.Xaml.Data;

namespace RandList.Helpers;

public class StringEqualsConverter : IValueConverter
{
    public StringEqualsConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || parameter == null)
        {
            return false;
        }
        else
        {
            return value.ToString()!.Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase);
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return parameter;
    }
}

