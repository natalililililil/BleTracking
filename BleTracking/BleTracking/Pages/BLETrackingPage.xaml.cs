using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Events;
using Plugin.BluetoothClassic.Abstractions;
using BleTracking.ViewModel;
using BleTracking.Models;

namespace BleTracking.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BLETrackingPage : ContentPage
    {
        private readonly IBluetoothAdapter _bluetoothAdapter;
        private BluetoothDeviceModel microcontrollerESP = new BluetoothDeviceModel("C8:F0:9E:51:40:DA", "CL-BLE-40D8");
        public BLETrackingPage()
        {
            //Task.Run(() => ShowReceiveData());
            _bluetoothAdapter = DependencyService.Resolve<IBluetoothAdapter>();
            InitializeComponent();                       
        }

        private async Task ShowReceiveData()
        {
            collectionView.ItemsSource = await App.BLETrackingDB.GetDevicesAsync();
        }
        private void RefreshUI()
        {
            if (_bluetoothAdapter.Enabled)
            {
                
                //btnDisableBluetooth.IsEnabled = true;
                //btnEnableBluetooth.IsEnabled = false;
                //lvBluetoothBoundedDevices.ItemsSource = _bluetoothAdapter.BondedDevices;
            }
            else
            {
                //btnDisableBluetooth.IsEnabled = false;
                //btnEnableBluetooth.IsEnabled = true;
                //lvBluetoothBoundedDevices.ItemsSource = null;
            }
        }

        protected override async void OnAppearing()
        {
            if (!_bluetoothAdapter.Enabled)
            {
                bool result = await DisplayAlert("Приложению нужен bluetooth для работы", "Хотите включить его?", "Да", "Нет");
                if (result)
                    _bluetoothAdapter.Enable();
            }
            collectionView.ItemsSource = await App.BLETrackingDB.GetDevicesAsync();

            RefreshUI();
            await DisconnectIfConnectedAsync();
        }

        private async Task DisconnectIfConnectedAsync()
        {
            if (App.CurrentBluetoothConnection != null)
            {
                try
                {
                    App.CurrentBluetoothConnection.Dispose();
                }
                catch (Exception exception)
                {
                    await DisplayAlert("Error", exception.Message, "Close");
                }
            }
        }

        private async Task ConnectToESP()
        {
            var connected = await TryConnect(microcontrollerESP);

            if (connected)
                await Navigation.PushAsync(new TerminalPage());
        }

        private async Task<bool> TryConnect(BluetoothDeviceModel bluetoothDeviceModel)
        {
            const bool Connected = true;
            const bool NotConnected = false;


            var connection = _bluetoothAdapter.CreateManagedConnection(bluetoothDeviceModel);
            try
            {
                connection.Connect();
                App.CurrentBluetoothConnection = connection;

                return Connected;
            }
            catch (BluetoothConnectionException exception)
            {
                await DisplayAlert("Connection error",
                    $"Can not connect to the device: {bluetoothDeviceModel.Name}({bluetoothDeviceModel.Address}).\n" +
                        $"Exception: \"{exception.Message}\"\n" +
                        "Please, try another one.",
                    "Close");

                return NotConnected;
            }
            catch (Exception exception)
            {
                await DisplayAlert("Generic error", exception.Message, "Close");

                return NotConnected;
            }

        }

        private async void Loader_Clicked(object sender, EventArgs e)
        {           
            ConnectToESP();
            //var loadingPage = new ConnectionPage();

            //await PopupNavigation.Instance.PushAsync(loadingPage);
            //await Task.Delay(2000);
            //await PopupNavigation.Instance.RemovePageAsync(loadingPage);
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                DeviceModel device = e.CurrentSelection.FirstOrDefault() as DeviceModel;

                if (device!= null)
                {
                    var connected = await TryConnect(microcontrollerESP);

                    if (connected)
                        await Navigation.PushAsync(new CurrentDevicePage(device.Address, device.Id));
                }               
                //await Shell.Current.GoToAsync(
                //    $"{nameof(CurrentDevicePage)}?{nameof(CurrentDevicePage.ItemId)}={device.Id.ToString()}");
            }
        }
        
    }
}