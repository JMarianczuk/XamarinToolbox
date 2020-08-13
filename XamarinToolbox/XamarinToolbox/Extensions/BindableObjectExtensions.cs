using Xamarin.Forms;
using XamarinToolbox.Helpers;

namespace XamarinToolbox.Extensions
{
    public static class BindableObjectExtensions
    {
        public static TProperty GetValue<TProperty>(this BindableObject obj, PropertyBinding<TProperty> propertyBinding)
        {
            return (TProperty) obj.GetValue(propertyBinding.Property);
        }

        public static void SetValue<TProperty>(this BindableObject obj, PropertyBinding<TProperty> propertyBinding,
            TProperty value)
        {
            obj.SetValue(propertyBinding.Property, value);
        }
    }
}