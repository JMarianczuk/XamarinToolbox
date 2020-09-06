using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinToolbox.Test.Model;

namespace XamarinToolbox.Test
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                SomeList.Add("four");
            });
            PropertyChanged += MainPage_PropertyChanged;
        }

        private void MainPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(EnumProperty):
                    int p = 5;
                    break;
            }
        }

        public ObservableCollection<string> SomeList { get; set; } = new ObservableCollection<string>()
        {
            "one",
            "two",
            "three",
        };

        public MethodClass MClass { get; set; } = new MethodClass();
        public string SomeText { get; set; } = "Test";
        private SomeEnum _enumProperty = SomeEnum.One;

        public SomeEnum EnumProperty
        {
            get => _enumProperty;
            set
            {
                if (_enumProperty != value)
                {
                    _enumProperty = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public enum SomeEnum
    {
        Zero,
        One,
        Two
    }
}
