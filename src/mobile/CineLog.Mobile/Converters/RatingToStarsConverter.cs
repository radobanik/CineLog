using System.Globalization;
using CineLog.Mobile.Helpers;

namespace CineLog.Mobile.Converters;

public class RatingToStarsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double rating)
            return string.Empty;

        var fullStars = (int)Math.Floor(rating);
        var hasHalf = (rating - fullStars) >= 0.5;

        return new string(FontAwesomeIcons.Star[0], fullStars) + (hasHalf ? FontAwesomeIcons.StarHalf : string.Empty);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
