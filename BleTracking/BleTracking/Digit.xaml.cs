namespace BleTracking
{
    using BleTracking.ViewModel;
    using Plugin.BluetoothClassic.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DigitPage : ContentPage
    {
        public List<byte> RecivedDataList { get; set; } = new List<byte>();
        public List<string> Users { get; set; }
        public string data = "";
        public DigitPage()
        {
            InitializeComponent();

            DigitViewModel model = (DigitViewModel)BindingContext;
            model.PropertyChanged += Model_PropertyChanged;

            if (App.CurrentBluetoothConnection != null)
            {
                App.CurrentBluetoothConnection.OnStateChanged += CurrentBluetoothConnection_OnStateChanged;
                App.CurrentBluetoothConnection.OnRecived += CurrentBluetoothConnection_OnRecived;
                App.CurrentBluetoothConnection.OnError += CurrentBluetoothConnection_OnError;
            }

            //Users = new List<string> { "Tom", "Bob", "Sam", "Alice" };
            //BindingContext = this;

        }

        ~DigitPage()
        {
            if (App.CurrentBluetoothConnection != null)
            {
                App.CurrentBluetoothConnection.OnStateChanged -= CurrentBluetoothConnection_OnStateChanged;
                App.CurrentBluetoothConnection.OnRecived -= CurrentBluetoothConnection_OnRecived;
                App.CurrentBluetoothConnection.OnError -= CurrentBluetoothConnection_OnError;
            }
        }

        private void CurrentBluetoothConnection_OnStateChanged(object sender, StateChangedEventArgs stateChangedEventArgs)
        {
            var model = (DigitViewModel)BindingContext;
            if (model != null)
            {
                model.ConnectionState = stateChangedEventArgs.ConnectionState;
            }
        }

        private void CurrentBluetoothConnection_OnRecived(object sender, Plugin.BluetoothClassic.Abstractions.RecivedEventArgs recivedEventArgs)
        {
            DigitViewModel model = (DigitViewModel)BindingContext;
            
            if (model != null)
            {
                model.SetReciving();

                for (int index = 0; index < recivedEventArgs.Buffer.Length; index++)
                {
                    byte value = recivedEventArgs.Buffer.ToArray()[index];
                    byte[] valueArray = new byte[] {value};
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

            String decodedString = ascii.GetString(asciiNumber);
            return decodedString;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DigitViewModel.Properties.Digit.ToString())
            {
                TransmitCurrentDigit();
            }
        }

        private void CurrentBluetoothConnection_OnError(object sender, System.Threading.ThreadExceptionEventArgs errorEventArgs)
        {
            if (errorEventArgs.Exception is BluetoothDataTransferUnitException)
            {
                TransmitCurrentDigit();
            }
        }

        private void TransmitCurrentDigit()
        {
            //List<string> list = new List<string>();
            DigitViewModel model = (DigitViewModel)BindingContext;
            if (model != null && !model.Reciving)
            {
                //foreach (var item in model.Digit)
                //{
                //    App.CurrentBluetoothConnection.Transmit(new Memory<byte>(new byte[] { item }));
                //}
                

                //App.CurrentBluetoothConnection.Transmit(new Memory<byte>(new byte[] { model.Digit }));
                //App.CurrentBluetoothConnection.Transmit(new Memory<byte>(new byte[] { model.Digit }));
            }          
        }
    }
}