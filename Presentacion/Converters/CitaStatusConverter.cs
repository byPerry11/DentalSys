using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Presentacion.Converters
{
    public class CitaStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                // Normaliza el texto para evitar problemas de mayúsculas/minúsculas
                string statusLower = status.ToLower().Trim();

                switch (statusLower)
                {
                    case "confirmada":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")); // Green
                    case "pendiente":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F")); // Yellow/Orange
                    case "cancelada":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")); // Red
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
