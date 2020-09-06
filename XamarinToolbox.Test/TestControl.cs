using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Xamarin.Forms;

namespace XamarinToolbox.Test
{
    public class TestControl : Grid
    {
        public static readonly BindableProperty TestListProperty =
            BindableProperty.Create(nameof(TestList), typeof(IEnumerable), typeof(TestControl), propertyChanged: OnTestListChanged);
        public static readonly BindableProperty TestTextProperty = 
            BindableProperty.Create(nameof(TestText), typeof(string), typeof(TestControl), propertyChanged: OnTestTextChanged);

        private static void OnTestTextChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is TestControl self && oldvalue is string oldText && newvalue is string newText)
            {
                self.TestTextChanged(oldText, newText);
            }
        }

        public IEnumerable TestList
        {
            get => (IEnumerable) GetValue(TestListProperty);
            set => SetValue(TestListProperty, value);
        }

        public string TestText
        {
            get => (string) GetValue(TestTextProperty);
            set => SetValue(TestTextProperty, value);
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