using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace HeroTableParser.Converters
{
    /// <summary>
    /// Converts an integer score to a SolidColorBrush based on a color mapping dictionary.
    /// </summary>
    public class ScoreToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not int score)
                return new SolidColorBrush(Colors.Transparent);

            // Default color mapping for strength scores
            var colorMap = new Dictionary<int, Color>
            {
                { 2, Colors.Green },
                { 1, Colors.Blue },
                { 0, Colors.Gray },
                { -1, Colors.Orange },
                { -2, Colors.Red }
            };

            if (colorMap.TryGetValue(score, out var color))
            {
                return new SolidColorBrush(color);
            }

            return new SolidColorBrush(Colors.White);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
