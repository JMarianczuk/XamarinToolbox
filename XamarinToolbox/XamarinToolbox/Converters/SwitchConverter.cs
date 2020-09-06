using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinToolbox.Converters
{
    [ContentProperty(nameof(Cases))]
    public class SwitchConverter<TValue, TReturn> : NotifyPropertyChangedBase, IValueConverter
    {
        public IList<Case<TValue, TReturn>> Cases { get; }

        public DataTemplate Template { get; set; }

        public SwitchConverter()
        {
            var cases = new ObservableCollection<Case<TValue, TReturn>>();
            Cases = cases;
            cases.CollectionChanged += Cases_CollectionChanged;
        }

        private void Cases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    CheckCases(e.NewItems.Cast<Case<TValue, TReturn>>());
                    break;
            }
        }

        private void CheckCases(IEnumerable<Case<TValue, TReturn>> cases)
        {
            foreach (var caseStatement in cases)
            {
                if (!(caseStatement.Value is TValue))
                {
                    throw new ArgumentException($"{nameof(Case<TValue, TReturn>)}.{nameof(Case<TValue, TReturn>.Value)} is not of type {typeof(TValue)}.");
                }
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var matchingCase = Cases.FirstOrDefault(c => Equals(c.Value, value));
            if (matchingCase != null)
            {
                if (Template == null)
                {
                    return matchingCase.Return;
                }
                else
                {
                    var instance = (BindableObject) Template.CreateContent();
                    instance.BindingContext = matchingCase.Return;
                    return instance;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Template != null)
            {
                throw new InvalidOperationException(
                    $"Cannot retrieve contained value out of template for {nameof(ConvertBack)}");
            }
            var matchingCase = Cases.FirstOrDefault(c => Equals(c.Return, value));
            if (matchingCase != null)
            {
                return matchingCase.Value;
            }
            return null;
        }
    }

    [ContentProperty(nameof(Return))]
    public class Case<TValue, TReturn>
    {
        public TValue Value { get; set; }
        public TReturn Return { get; set; }
    }
}