using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Java.IO;
using Android.Bluetooth;
using System.Linq;
using Java.Util;
using System.Threading.Tasks;

namespace BleTracking.Droid
{
    [Activity(Label = "BLE Tracking", Icon = "@mipmap/icon9", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            ConnectAndGetDataAsync();
            LoadApplication(new App());
        }

        private async Task ConnectAndGetDataAsync()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth adapter is not enabled.");

            BluetoothDevice device = (from bd in adapter.BondedDevices
                                      where bd.Name == "CL-BLE-DDB8"
                                      select bd).FirstOrDefault();
            byte[] buffer = new byte[30];

            if (device == null)
                throw new Exception("Named device not found.");

            var _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            await _socket.ConnectAsync();

            await _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

            if (adapter == null)
                throw new Exception("No Bluetooth adapter found.");
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}