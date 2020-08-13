using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace XamarinToolbox.Converters
{
    public class MethodInvocationConverter<TClass, TResult> : IValueConverter
    {
        public string Method { get; set; }
        private MethodInfo _methodInfo;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TClass instance)
            {
                if (_methodInfo == null)
                {
                    var type = typeof(TClass);
                    _methodInfo = type.GetMethod(Method, BindingFlags.Instance | BindingFlags.Public);
                    if (_methodInfo.ReturnType != typeof(TResult))
                    {
                        throw new ArgumentException($"Method {Method} does not have return type {typeof(TResult)}");
                    }
                }
                if (parameter != null)
                {
                    return (TResult) _methodInfo.Invoke(instance, new[] {parameter});
                }
                else
                {
                    return (TResult) _methodInfo.Invoke(instance, new object[] { });
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}