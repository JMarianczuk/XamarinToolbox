using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinToolbox.MarkupExtensions
{
    [ContentProperty(nameof(Method))]
    public class StaticMethodExtension : IMarkupExtension
    {
        public string Method { get; set; }

        [TypeConverter(typeof(TypeTypeConverter))]
        public Type Type { get; set; }

        private object _func;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_func == null)
            {
                if (string.IsNullOrWhiteSpace(Method))
                {
                    throw new ArgumentException("Missing Method");
                }
                if (Type == null)
                {
                    throw new ArgumentException("Missing Type");
                }
                var reflectedFunc = Type.GetMethod(Method,
                    BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                if (reflectedFunc == null)
                {
                    throw new ArgumentException($"Method {Method} not found on type {Type}");
                }
                if (reflectedFunc.ReturnType == typeof(void))
                {
                    throw new ArgumentException(
                        $"Method {Method} of type {Type} may not return result of type {typeof(void)}");
                }
                var resultType = reflectedFunc.ReturnType;
                var parameters = reflectedFunc.GetParameters();
                if (parameters.Length > 16)
                {
                    throw new ArgumentException(
                        $"Method {Method} of type {Type} cannot take more than 16 parameters");
                }
                var funcType = GetFuncType(parameters.Length);
                var typeParameters = parameters.Select(x => x.ParameterType).Append(resultType).ToArray();
                var genericType = funcType.MakeGenericType(typeParameters);
                _func = Delegate.CreateDelegate(genericType, reflectedFunc, true);
            }
            return _func;
        }

        private static Type GetFuncType(int numberOfParameters)
        {
            switch (numberOfParameters)
            {
                case 0:
                    return typeof(Func<>);
                case 1:
                    return typeof(Func<,>);
                case 2:
                    return typeof(Func<,,>);
                case 3:
                    return typeof(Func<,,,>);
                case 4:
                    return typeof(Func<,,,,>);
                case 5:
                    return typeof(Func<,,,,,>);
                case 6:
                    return typeof(Func<,,,,,,>);
                case 7:
                    return typeof(Func<,,,,,,,>);
                case 8:
                    return typeof(Func<,,,,,,,,>);
                case 9:
                    return typeof(Func<,,,,,,,,,>);
                case 10:
                    return typeof(Func<,,,,,,,,,,>);
                case 11:
                    return typeof(Func<,,,,,,,,,,,>);
                case 12:
                    return typeof(Func<,,,,,,,,,,,,>);
                case 13:
                    return typeof(Func<,,,,,,,,,,,,,>);
                case 14:
                    return typeof(Func<,,,,,,,,,,,,,,>);
                case 15:
                    return typeof(Func<,,,,,,,,,,,,,,,>);
                case 16:
                    return typeof(Func<,,,,,,,,,,,,,,,,>);
                default:
                    throw new ArgumentException($"Cannot create Func for more than 16 parameters");
            }
        }
    }
}