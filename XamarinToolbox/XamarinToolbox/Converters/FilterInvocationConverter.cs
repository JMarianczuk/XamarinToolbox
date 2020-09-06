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
                IEnumerable<TElement> resultEnumerable;
                if (sequence is INotifyCollectionChanged incc)
                {
                    resultEnumerable = WrapCollectionChanged(sequence, incc, Predicate);
                }
                else
                {
                    resultEnumerable = sequence.Where(Predicate);
                }
                return resultEnumerable;
            }
            return Enumerable.Empty<TElement>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> WrapCollectionChanged(IEnumerable<TElement> enumerable, INotifyCollectionChanged incc, Func<TElement, bool> predicate)
        {
            return new CollectionChangedWrapper(enumerable, incc, predicate);
        }

        private class CollectionChangedWrapper : IEnumerable<TElement>, INotifyCollectionChanged
        {
            private readonly IEnumerable<TElement> _source;
            private readonly Func<TElement, bool> _predicate;
            private readonly List<bool> _bitMask;
            private int _trueCount;
            public CollectionChangedWrapper(IEnumerable<TElement> source, INotifyCollectionChanged originalChangingCollection, Func<TElement, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
                originalChangingCollection.CollectionChanged += OnOriginalCollectionChanged;
                _bitMask = source.Select(predicate).ToList();
                _trueCount = _bitMask.Count(TrueValues);
            }
            public IEnumerator<TElement> GetEnumerator()
            {
                return _source.Where(_predicate).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static bool TrueValues(bool value) => value;
            private static Func<TElement, int, bool> IsMasked(bool[] mask) => (x, i) => mask[i];

            private void OnOriginalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e == null)
                {
                    return;
                }
                NotifyCollectionChangedEventArgs args = null;
                int index;
                int filteredIndex;

                int addAmount;
                int removeAmount;
                bool[] addMask;
                bool[] removeMask;

                IList<TElement> addItems;

                var newItems = e.NewItems?.Cast<TElement>() ?? Enumerable.Empty<TElement>();
                var oldItems = e.OldItems?.Cast<TElement>() ?? Enumerable.Empty<TElement>();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        filteredIndex = _bitMask.Take(index).Count(TrueValues);
                        addItems = newItems.ToList();
                        addMask = addItems.Select(_predicate).ToArray();
                        if (addMask.Any(TrueValues))
                        {
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addItems.Where(IsMasked(addMask)).ToList(), filteredIndex);
                            _trueCount += addMask.Count(TrueValues);
                        }
                        _bitMask.InsertRange(index, addMask);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        index = e.OldStartingIndex;
                        removeAmount = e.OldItems.Count;
                        removeMask = _bitMask.Skip(index).Take(removeAmount).ToArray();
                        if (removeMask.Any(TrueValues))
                        {
                            filteredIndex = _bitMask.Take(index).Count(TrueValues);
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                oldItems.Where(IsMasked(removeMask)).ToList(), filteredIndex);
                            ;
                            _trueCount -= removeMask.Count(TrueValues);
                        }
                        _bitMask.RemoveRange(index, removeAmount);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        index = e.OldStartingIndex;
                        removeAmount = e.OldItems.Count;
                        addAmount = e.NewItems.Count;
                        removeMask = _bitMask.Skip(index).Take(removeAmount).ToArray();
                        addItems = newItems.ToList();
                        addMask = addItems.Select(_predicate).ToArray();
                        var oldRelevant = removeMask.Any(TrueValues);
                        var newRelevant = addMask.Any(TrueValues);
                        filteredIndex = _bitMask.Take(index).Count(TrueValues);
                        if (oldRelevant && newRelevant)
                        {
                            args = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                addItems.Where(IsMasked(addMask)).ToList(),
                                oldItems.Where(IsMasked(removeMask)).ToList(), 
                                filteredIndex);
                            _trueCount += (addMask.Count(TrueValues) - removeMask.Count(TrueValues));
                        }
                        else if (oldRelevant && !newRelevant)
                        {
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                oldItems.Where(IsMasked(removeMask)).ToList(), filteredIndex);
                            _trueCount -= removeMask.Count(TrueValues);
                        }
                        else if (!oldRelevant && newRelevant)
                        {
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                addItems.Where(IsMasked(addMask)).ToList(), filteredIndex);
                            _trueCount += addMask.Count(TrueValues);
                        }
                        _bitMask.RemoveRange(index, removeAmount);
                        _bitMask.InsertRange(index, addMask);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        var oldIndex = e.OldStartingIndex;
                        var newIndex = e.NewStartingIndex;
                        var moveAmount = e.OldItems.Count;
                        var moveMask = _bitMask.Skip(oldIndex).Take(moveAmount).ToArray();
                        if (moveMask.Any(TrueValues))
                        {
                            var filteredOldIndex = _bitMask.Take(oldIndex).Count(TrueValues);
                            var filteredNewIndex = _bitMask.Take(newIndex).Count(TrueValues);
                            if (filteredOldIndex != filteredNewIndex)
                            {
                                args = new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Move,
                                    oldItems.Where(IsMasked(moveMask)).ToList(),
                                    filteredNewIndex,
                                    filteredOldIndex);
                            }
                        }
                        _bitMask.RemoveRange(oldIndex, moveAmount);
                        _bitMask.InsertRange(newIndex, moveMask);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _bitMask.Clear();
                        _trueCount = 0;
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                        break;
                }

                if (args != null)
                {
                    CollectionChanged?.Invoke(sender, args);
                }
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
        }
    }
}