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
                var parameters = reflectedFunc.GetParameters();
                if (parameters.Length != 1)
                {
                    throw new ArgumentException(
                        $"Method {Method} of type {Type} needs to take exactly one parameter");
                }
                var parameterType = parameters.First().ParameterType;
                var resultType = reflectedFunc.ReturnType;
                var funcType = typeof(Func<,>);
                var genericType = funcType.MakeGenericType(parameterType, resultType);
                _func = Delegate.CreateDelegate(genericType, reflectedFunc, true);
            }
            return _func;
        }
    }
}