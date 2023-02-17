using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BleTracking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BLEListPage : ContentPage
    {
        public BLEListPage()
        {
            InitializeComponent();
        }
        private async void SwitchToTerminal_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DigitPage());
        }
    }
}