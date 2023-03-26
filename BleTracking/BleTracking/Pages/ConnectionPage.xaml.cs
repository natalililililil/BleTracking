using Rg.Plugins.Popup.Pages;

using Plugin.BluetoothClassic.Abstractions;

using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;

namespace BleTracking.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionPage : PopupPage
    {
        public ConnectionPage()
        {
            InitializeComponent();
        }       
    }
}