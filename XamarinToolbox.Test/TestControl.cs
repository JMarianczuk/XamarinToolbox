using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Xamarin.Forms;
using XamarinToolbox.Extensions;
using XamarinToolbox.Helpers;

namespace XamarinToolbox.Test
{
    public class TestControl : Grid
    {
        public static readonly BindableProperty TestListProperty =
            BindableProperty.Create(nameof(TestList), typeof(IEnumerable), typeof(TestControl), propertyChanged: OnTestListChanged);
        public static readonly PropertyBinding<string> TestTextBinding =
            PropertyBinding.Create<string, TestControl>(nameof(TestText), propertyChanged: self => self.TestTextChanged);
        public static readonly BindableProperty TestTextProperty = TestTextBinding.Property;

        public IEnumerable TestList
        {
            get => (IEnumerable) GetValue(TestListProperty);
            set => SetValue(TestListProperty, value);
        }

        public string TestText
        {
            get => this.GetValue(TestTextBinding);
            set => this.SetValue(TestTextBinding, value);
        }

        private Label _text;

        public TestControl()
        {
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            _text = new Label();
            Children.Add(_text, 0, 0);
        }

        private static void OnTestListChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is TestControl self)
            {
                self.OnTestListChanged();
            }
        }
        public void OnTestListChanged()
        {
            if (TestList is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += TestListCollectionChanged;
            }
        }

        private void TestListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }
        private static void OnTestTextChanged(TestControl self, string oldValue, string newValue)
        {
            self.TestTextChanged(oldValue, newValue);
        }

        private void TestTextChanged(string oldValue, string newValue)
        {
            _text.Text = TestText;
        }
    }
}