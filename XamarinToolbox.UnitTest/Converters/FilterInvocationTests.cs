using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamarinToolbox.Converters;
using XamarinToolbox.UnitTest.FluentExtensions;

namespace XamarinToolbox.UnitTest.Converters
{
    [TestClass]
    public class FilterInvocationTests : ConverterTestBase
    {
        private ObservableCollection<int> _collection;

        private IEnumerable<int> _filteredResult;

        private List<NotifyCollectionChangedEventArgs> _collectionChangedEvents;

        public void AnEmptyCollection() => _collection = new ObservableCollection<int>();

        public void NumbersFromOneToTen()
        {
            _collection = new ObservableCollection<int>()
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10
            };
        }
        public void APassingFilter()
        {
            Converter = new FilterInvocationConverter<int>()
            {
                Predicate = number => true,
            };
        }

        public void ABlockingFilter()
        {
            Converter = new FilterInvocationConverter<int>()
            {
                Predicate = number => false,
            };
        }

        public void AFilterThatBlocksOddNumbers()
        {
            Converter = new FilterInvocationConverter<int>()
            {
                Predicate = number => number % 2 == 0,
            };
        }

        public void TheCollectionIsFiltered() => _filteredResult = (IEnumerable<int>) Converter.Convert(_collection, typeof(IEnumerable<int>), null, CultureInfo.InvariantCulture);

        public void ACollectionChangedEventIsSubscribedTo()
        {
            _collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
            var incc = (INotifyCollectionChanged) _filteredResult;
            incc.CollectionChanged += (sender, e) => _collectionChangedEvents.Add(e);
        }

        public Action AValueIsAddedToTheCollection(int index, int value) => () => _collection.Insert(index, value);
        public Action AnOddValueIsAddedToTheCollection(int index, int value)
        {
            value.Should().BeOdd();
            return () =>
            {
                _collection.Insert(index, value);
            };
        }

        public Action AValueIsRemovedFromTheCollection(int index) => () => _collection.RemoveAt(index);
        public Action AnOddValueIsRemovedFromTheCollection(int index) => () =>
        {
            _collection[index].Should().BeOdd();
            _collection.RemoveAt(index);
        };

        public Action AValueIsReplacedFromTheCollection(int index, int newValue) => () => _collection[index] = newValue;
        public Action AnEvenValueIsReplacedByAnOddValueFromTheCollection(int index, int newValue)
        {
            newValue.Should().BeOdd();
            return () =>
            {
                _collection[index].Should().BeEven();
                _collection[index] = newValue;
            };
        }
        public Action AnOddValueIsReplacedByAnEvenValueFromTheCollection(int index, int newValue)
        {
            newValue.Should().BeEven();
            return () =>
            {
                _collection[index].Should().BeOdd();
                _collection[index] = newValue;
            };
        }

        public Action AnOddValueIsReplacedByAnOddValueFromTheCollection(int index, int newValue)
        {
            newValue.Should().BeOdd();
            return () =>
            {
                _collection[index].Should().BeOdd();
                _collection[index] = newValue;
            };
        }

        public Action AValueIsMovedWithinTheCollection(int oldIndex, int newIndex) => () => _collection.Move(oldIndex, newIndex);
        public Action AnEvenValueIsMovedWithinTheCollection(int oldIndex, int newIndex) => () =>
        {
            _collection[oldIndex].Should().BeEven();
            _collection.Move(oldIndex, newIndex);
        };
        public Action AnOddValueIsMovedWithinTheCollection(int oldIndex, int newIndex) => () =>
        {
            _collection[oldIndex].Should().BeOdd();
            _collection.Move(oldIndex, newIndex);
        };

        public void TheCollectionIsCleared() => _collection.Clear();
        public void TheResultIsEmpty() => _filteredResult.Should().BeEmpty();
        public Action TheResultIs(params int[] numbers) => () => _filteredResult.Should().Equal(numbers);
        public void TheResultImplements<TType>() => _filteredResult.Should().BeAssignableTo<TType>();
        public void ResultAndSourceAreEqual() => _filteredResult.Should().Equal(_collection);
        public void OneEventWasFired() => _collectionChangedEvents.Should().ContainSingle();
        public void NoEventWasFired() => _collectionChangedEvents.Should().BeEmpty();

        public Action TheEventMatches(NotifyCollectionChangedAction action, int? oldStartingIndex = null,
            int? newStartingIndex = null, int[] oldItems = null, int[] newItems = null)
        {
            return () =>
            {
                var changedEvent = _collectionChangedEvents.Single();
                changedEvent.Action.Should().Be(action);
                if (oldStartingIndex != null)
                {
                    changedEvent.OldStartingIndex.Should().Be(oldStartingIndex.Value);
                }
                if (newStartingIndex != null)
                {
                    changedEvent.NewStartingIndex.Should().Be(newStartingIndex.Value);
                }
                if (oldItems != null)
                {
                    changedEvent.OldItems.Cast<int>().Should().Equal(oldItems);
                }
                if (newItems != null)
                {
                    changedEvent.NewItems.Cast<int>().Should().Equal(newItems);
                }
            };
        }

        [TestMethod]
        public void EmptyCollectionTests()
        {
            Given(AnEmptyCollection, APassingFilter);
            When(TheCollectionIsFiltered);
            Then(TheResultIsEmpty);

            Given(AnEmptyCollection, ABlockingFilter);
            When(TheCollectionIsFiltered);
            Then(TheResultIsEmpty);
        }

        [TestMethod]
        public void BlockingFilterTest()
        {
            Given(NumbersFromOneToTen, ABlockingFilter);
            When(TheCollectionIsFiltered);
            Then(TheResultIsEmpty);
        }

        [TestMethod]
        public void PassingFilterTest()
        {
            Given(NumbersFromOneToTen, APassingFilter);
            When(TheCollectionIsFiltered);
            Then(ResultAndSourceAreEqual);
        }

        [TestMethod]
        public void BasicFilterTest()
        {
            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered);
            Then(TheResultIs(2, 4, 6, 8, 10));
        }

        [TestMethod]
        public void BasicCollectionChangedTest()
        {
            Given(AnEmptyCollection, APassingFilter);
            When(TheCollectionIsFiltered);
            Then(TheResultImplements<INotifyCollectionChanged>);
        }

        [TestMethod]
        public void BasicAddEventTest()
        {
            Given(NumbersFromOneToTen, APassingFilter);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AValueIsAddedToTheCollection(10, 11));
            Then(OneEventWasFired, TheEventMatches(NotifyCollectionChangedAction.Add, newStartingIndex: 10, newItems: new[]{11}));
        }

        [TestMethod]
        public void BasicRemoveEventTest()
        {
            Given(NumbersFromOneToTen, APassingFilter);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AValueIsRemovedFromTheCollection(3));
            Then(OneEventWasFired, 
                TheEventMatches(NotifyCollectionChangedAction.Remove, oldStartingIndex: 3, oldItems: new[] {4}));
        }

        [TestMethod]
        public void BasicReplaceEventTest()
        {
            Given(NumbersFromOneToTen, APassingFilter);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AValueIsReplacedFromTheCollection(5, 24));
            Then(OneEventWasFired,
                TheEventMatches(NotifyCollectionChangedAction.Replace, oldStartingIndex: 5, newStartingIndex: 5, oldItems: new[] { 6 }, newItems: new[] { 24 }));
        }

        [TestMethod]
        public void BasicMoveEventTest()
        {
            Given(NumbersFromOneToTen, APassingFilter);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AValueIsMovedWithinTheCollection(3, 7));
            Then(OneEventWasFired,
                TheEventMatches(NotifyCollectionChangedAction.Move, oldStartingIndex: 3, newStartingIndex: 7, oldItems: new[] { 4 }));
        }

        [TestMethod]
        public void BasicResetEventTest()
        {
            Given(NumbersFromOneToTen, APassingFilter);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, TheCollectionIsCleared);
            Then(OneEventWasFired, TheEventMatches(NotifyCollectionChangedAction.Reset));
        }

        [TestMethod]
        public void FilteredAddEventTest()
        {
            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AnOddValueIsAddedToTheCollection(4, 17));
            Then(NoEventWasFired);
        }

        [TestMethod]
        public void FilteredRemoveEventTest()
        {
            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AnOddValueIsRemovedFromTheCollection(0));
            Then(NoEventWasFired);
        }

        [TestMethod]
        public void FilteredReplaceEventTests()
        {
            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AnOddValueIsReplacedByAnOddValueFromTheCollection(2, 31));
            Then(NoEventWasFired);

            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AnOddValueIsReplacedByAnEvenValueFromTheCollection(2, 30));
            Then(OneEventWasFired,
                TheEventMatches(NotifyCollectionChangedAction.Add, newStartingIndex: 1, newItems: new[] { 30 }));

            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo, AnEvenValueIsReplacedByAnOddValueFromTheCollection(5, 31));
            Then(OneEventWasFired,
                TheEventMatches(NotifyCollectionChangedAction.Remove, oldStartingIndex: 2, oldItems: new[] { 6 }));
        }

        [TestMethod]
        public void FilteredMoveEventTests()
        {
            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo,
                AnOddValueIsMovedWithinTheCollection(4, 8));
            Then(NoEventWasFired);

            Given(NumbersFromOneToTen, AFilterThatBlocksOddNumbers);
            When(TheCollectionIsFiltered, ACollectionChangedEventIsSubscribedTo,
                AnEvenValueIsMovedWithinTheCollection(3, 6));
            Then(OneEventWasFired,
                TheEventMatches(NotifyCollectionChangedAction.Move, oldStartingIndex: 1, newStartingIndex: 3,
                    oldItems: new[] { 4 }));
        }
    }
}
