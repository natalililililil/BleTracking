using BleTracking.ViewModel;
using Plugin.BluetoothClassic.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BleTracking.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TerminalPage : ContentPage
    {
        BluetoothDeviceModel bluetoothDeviceModel;
        public TerminalPage(BluetoothDeviceModel bluetoothDeviceModel)
        {
            InitializeComponent();

            if (App.CurrentBluetoothConnection != null)
            {
                App.CurrentBluetoothConnection.OnRecived += CurrentBluetoothConnection_OnRecived;
            }

            this.bluetoothDeviceModel = bluetoothDeviceModel;
            //CheckConnection(bluetoothDeviceModel);
            
        }

        private async Task CheckConnection(BluetoothDeviceModel bluetoothDeviceModel)
        {
            if (bluetoothDeviceModel.Address == "C8:F0:9E:51:40:DA")
                await Navigation.PushAsync(new BLEListPage());
        }
        ~TerminalPage()
        {
            App.CurrentBluetoothConnection.OnRecived -= CurrentBluetoothConnection_OnRecived;
        }

        private void CurrentBluetoothConnection_OnRecived(object sender, Plugin.BluetoothClassic.Abstractions.RecivedEventArgs recivedEventArgs)
        {
            //var terminal = new TerminalPage();
            //terminal.CurrentBluetoothConnection_OnRecived(sender, recivedEventArgs);

            //await Navigation.PushAsync(new TerminalPage());

            DigitViewModel model = (DigitViewModel)BindingContext;

            if (model != null)
            {
                model.SetReciving();

                for (int index = 0; index < recivedEventArgs.Buffer.Length; index++)
                {
                    byte value = recivedEventArgs.Buffer.ToArray()[index];
                    byte[] valueArray = new byte[] { value };
                    model.Digit += ConvertASCIIToString(valueArray);
                    //data += value.ToString();
                    //RecivedDataList.Add(value);
                    //model.Digit += " " + value.ToString();

                }

                model.SetRecived();
            }
        }

        private string ConvertASCIIToString(byte[] asciiNumber)
        {
            Encoding ascii = Encoding.ASCII;
            return ascii.GetString(asciiNumber);
        }
    }
}