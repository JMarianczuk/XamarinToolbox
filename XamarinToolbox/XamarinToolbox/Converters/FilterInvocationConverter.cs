using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace XamarinToolbox.Converters
{
    [ContentProperty(nameof(Predicate))]
    public class FilterInvocationConverter<TElement> : IValueConverter
    {
        public Func<TElement, bool> Predicate { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<TElement> sequence)
            {
                var resultEnumerable = sequence.Where(Predicate);
                if (sequence is INotifyCollectionChanged incc)
                {
                    resultEnumerable = WrapCollectionChanged(resultEnumerable, incc);
                }
                return resultEnumerable;
            }
            return Enumerable.Empty<TElement>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> WrapCollectionChanged(IEnumerable<TElement> enumerable, INotifyCollectionChanged incc)
        {
            return new CollectionChangedWrapper(enumerable, incc);
        }

        private class CollectionChangedWrapper : IEnumerable<TElement>, INotifyCollectionChanged
        {
            private readonly IEnumerable<TElement> _source;
            public CollectionChangedWrapper(IEnumerable<TElement> source, INotifyCollectionChanged originalChangingCollection)
            {
                _source = source;
                originalChangingCollection.CollectionChanged += OnInvokeCollectionChanged;
            }
            public IEnumerator<TElement> GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private void OnInvokeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(sender, e);
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
        }
    }
}