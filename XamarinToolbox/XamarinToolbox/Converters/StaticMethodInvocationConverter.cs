using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamarinToolbox.Converters
{
    public class StaticMethodInvocationConverter<TInput, TOutput> : IValueConverter
    {
        public Func<TInput, TOutput> InvocationFunc { get; set; }
        public StaticMethodInvocationConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TInput input)
            {
                return InvocationFunc(input);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}