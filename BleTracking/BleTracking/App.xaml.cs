using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BluetoothClassic.Abstractions;
using BleTracking.Data;
using System.IO;

namespace BleTracking
{
    public partial class App : Application
    {
        static BLETrackingDB db;

        public static BLETrackingDB BLETrackingDB
        {
            get
            {
                if (db == null)
                {
                    db = new BLETrackingDB(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BLETrackingDB.db3"));
                }
                return db;
            }
        }
        public static IBluetoothManagedConnection CurrentBluetoothConnection { get; internal set; }
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
            //MainPage = new NavigationPage(new SelectBluetoothRemoteDevicePage());
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
