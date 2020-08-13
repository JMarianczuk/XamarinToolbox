using System;
using Xamarin.Forms;

namespace XamarinToolbox.Helpers
{
    public class PropertyBinding<TProperty>
    {
        public delegate bool ValidateValueDelegate<TDeclaringType>(TDeclaringType self, TProperty value);
        public delegate void BindablePropertyChangedDelegate<TDeclaringType>(TDeclaringType self, TProperty oldValue, TProperty newValue);
        public delegate void BindablePropertyChangingDelegate<TDeclaringType>(TDeclaringType self, TProperty oldValue, TProperty newValue);
        public delegate TProperty CoerceValueDelegate<TDeclaringType>(TDeclaringType self, TProperty value);
        public delegate TProperty CreateDefaultValueDelegate<TDeclaringType>(TDeclaringType self);

        public delegate bool ValidateValue(TProperty value);
        public delegate void BindablePropertyChanged(TProperty oldValue, TProperty newValue);
        public delegate void BindablePropertyChanging(TProperty oldValue, TProperty newValue);
        public delegate TProperty CoerceValue(TProperty value);
        public delegate TProperty CreateDefaultValue();
        public BindableProperty Property { get; }

        public PropertyBinding(BindableProperty property)
        {
            Property = property;
        }
    }

    public class PropertyBinding
    {
        public static PropertyBinding<TProperty> Create<TProperty, TDeclaringType>(
            string propertyName,
            object defaultValue = null,
            BindingMode defaultBindingMode = BindingMode.OneWay,
            PropertyBinding<TProperty>.ValidateValueDelegate<TDeclaringType> validateValue = null,
            PropertyBinding<TProperty>.BindablePropertyChangedDelegate<TDeclaringType> propertyChanged = null,
            PropertyBinding<TProperty>.BindablePropertyChangingDelegate<TDeclaringType> propertyChanging = null,
            PropertyBinding<TProperty>.CoerceValueDelegate<TDeclaringType> coerceValue = null,
            PropertyBinding<TProperty>.CreateDefaultValueDelegate<TDeclaringType> defaultValueCreator = null)
            where TDeclaringType : BindableObject
        {
            BindableProperty.ValidateValueDelegate validateV = null;
            if (validateValue != null)
            {
                validateV = (bindable, value) => validateValue((TDeclaringType) bindable, (TProperty) value);
            }
            BindableProperty.BindingPropertyChangedDelegate pChanged = null;
            if (propertyChanged != null)
            {
                pChanged = (bindable, oldValue, newValue) => propertyChanged((TDeclaringType) bindable, (TProperty) oldValue, (TProperty) newValue);
            }
            BindableProperty.BindingPropertyChangingDelegate pChanging = null;
            if (propertyChanging != null)
            {
                pChanging = (bindable, oldValue, newValue) => propertyChanging((TDeclaringType) bindable, (TProperty) oldValue, (TProperty) newValue);
            }
            BindableProperty.CoerceValueDelegate cValue = null;
            if (coerceValue != null)
            {
                cValue = (bindable, value) => coerceValue((TDeclaringType) bindable, (TProperty) value);
            }
            BindableProperty.CreateDefaultValueDelegate dValCreator = null;
            if (defaultValueCreator != null)
            {
                dValCreator = bindable => defaultValueCreator((TDeclaringType) bindable);
            }
            return new PropertyBinding<TProperty>(
                BindableProperty.Create(
                    propertyName, 
                    typeof(TProperty), 
                    typeof(TDeclaringType),
                    defaultValue: defaultValue,
                    defaultBindingMode: defaultBindingMode,
                    validateValue: validateV,
                    propertyChanged: pChanged,
                    propertyChanging: pChanging,
                    coerceValue: cValue,
                    defaultValueCreator: dValCreator)
                );
        }
        public static PropertyBinding<TProperty> Create<TProperty, TDeclaringType>(
            string propertyName,
            object defaultValue = null,
            BindingMode defaultBindingMode = BindingMode.OneWay,
            Func<TDeclaringType, PropertyBinding<TProperty>.ValidateValue> validateValue = null,
            Func<TDeclaringType, PropertyBinding<TProperty>.BindablePropertyChanged> propertyChanged = null,
            Func<TDeclaringType, PropertyBinding<TProperty>.BindablePropertyChanging> propertyChanging = null,
            Func<TDeclaringType, PropertyBinding<TProperty>.CoerceValue> coerceValue = null,
            Func<TDeclaringType, PropertyBinding<TProperty>.CreateDefaultValue> defaultValueCreator = null)
            where TDeclaringType : BindableObject
        {
            BindableProperty.ValidateValueDelegate validateV = null;
            if (validateValue != null)
            {
                validateV = (bindable, value) => validateValue((TDeclaringType) bindable)((TProperty) value);
            }
            BindableProperty.BindingPropertyChangedDelegate pChanged = null;
            if (propertyChanged != null)
            {
                pChanged = (bindable, oldValue, newValue) => propertyChanged((TDeclaringType) bindable)((TProperty) oldValue, (TProperty) newValue);
            }
            BindableProperty.BindingPropertyChangingDelegate pChanging = null;
            if (propertyChanging != null)
            {
                pChanging = (bindable, oldValue, newValue) => propertyChanging((TDeclaringType) bindable)((TProperty) oldValue, (TProperty) newValue);
            }
            BindableProperty.CoerceValueDelegate cValue = null;
            if (coerceValue != null)
            {
                cValue = (bindable, value) => coerceValue((TDeclaringType) bindable)((TProperty) value);
            }
            BindableProperty.CreateDefaultValueDelegate dValCreator = null;
            if (defaultValueCreator != null)
            {
                dValCreator = bindable => defaultValueCreator((TDeclaringType) bindable);
            }
            return new PropertyBinding<TProperty>(
                BindableProperty.Create(
                    propertyName,
                    typeof(TProperty),
                    typeof(TDeclaringType),
                    defaultValue: defaultValue,
                    defaultBindingMode: defaultBindingMode,
                    validateValue: validateV,
                    propertyChanged: pChanged,
                    propertyChanging: pChanging,
                    coerceValue: cValue,
                    defaultValueCreator: dValCreator)
                );
        }
    }
}