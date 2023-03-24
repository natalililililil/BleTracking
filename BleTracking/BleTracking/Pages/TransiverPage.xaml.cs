using Plugin.BluetoothClassic.Abstractions;
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
    public partial class TransiverPage : ContentPage
    {
        public TransiverPage()
        {
            InitializeComponent();
        }

        private async void btnTransmit_Clicked(object sender, EventArgs e)
        {
            const int BufferSize = 1;
            const int OffsetDefault = 0;
            var device = (BluetoothDeviceModel)BindingContext;

            if (device != null)
            {
                var adapter = DependencyService.Resolve<IBluetoothAdapter>();

                using (var connection = adapter.CreateConnection(device))
                {
                    if (await connection.RetryConnectAsync(retriesCount: 2))
                    {
                       // byte[] buffer = new byte[BufferSize] { (byte)stepperDigit.Value };
                        byte[] buffer = new byte[BufferSize] ;
                        if (!await connection.RetryTransmitAsync(buffer, OffsetDefault, buffer.Length))
                        {
                            await DisplayAlert("Error", "Can't send data.", "Close");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Can't to connect.", "Close");
                    }
                }
            }
        }


        private async void btnRecive_Clicked(object sender, EventArgs e)
        {
            
            const int BufferSize = 3;
            const int OffsetDefault = 0;
            var device = (BluetoothDeviceModel)BindingContext;

            if (device != null)
            {
                var adapter = DependencyService.Resolve<IBluetoothAdapter>();
                using (var connection = adapter.CreateConnection(device))
                {
                    if (await connection.RetryConnectAsync(retriesCount: 2))
                    {

                        //string[] buffer = new string[BufferSize];
                        byte[] buffer = new byte[BufferSize];
                        if (!(await connection.RetryReciveAsync(buffer, OffsetDefault, buffer.Length)).Succeeded)
                        {
                            await DisplayAlert("Error", "Can't recive data.", "Close");
                        }
                        else
                        {
                            getData.BindingContext = "hi";
                            //getData.Value = buffer.FirstOrDefault();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Can't to connect.", "Close");
                    }
                }
            }
        }


    }
}