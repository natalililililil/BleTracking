using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BluetoothClassic.Abstractions;

namespace BleTracking
{
    public partial class App : Application
    {
        public static IBluetoothManagedConnection CurrentBluetoothConnection { get; internal set; }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new SelectBluetoothRemoteDevicePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
