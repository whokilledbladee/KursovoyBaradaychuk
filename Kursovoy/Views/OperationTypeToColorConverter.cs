using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Kursovoy.Views
{
    public class OperationTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OperationType operationType)
            {
                return operationType == OperationType.Income
                    ? new SolidColorBrush(Color.FromRgb(39, 174, 96)) // Зеленый для доходов
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Красный для расходов
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}