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
        }

        public ObservableCollection<string> SomeList { get; set; } = new ObservableCollection<string>()
        {
            "one",
            "two",
            "three",
        };

        public MethodClass MClass { get; set; } = new MethodClass();
        public string SomeText { get; set; } = "Test";
    }
}
