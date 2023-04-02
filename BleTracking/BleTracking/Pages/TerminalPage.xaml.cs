using BleTracking.ESP32Data;
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
        private List<BLEDevice> bLEDevices = new List<BLEDevice>();
        StringBuilder receiveData = new StringBuilder();
        int numberOfReceiveCharUsed;
        
        public TerminalPage(BluetoothDeviceModel bluetoothDeviceModel)
        {
            InitializeComponent();

            if (App.CurrentBluetoothConnection != null)
            {
                App.CurrentBluetoothConnection.OnStateChanged += CurrentBluetoothConnection_OnStateChanged;
                App.CurrentBluetoothConnection.OnRecived += CurrentBluetoothConnection_OnRecived;
            }
        }

        ~TerminalPage()
        {
            App.CurrentBluetoothConnection.OnStateChanged -= CurrentBluetoothConnection_OnStateChanged;
            App.CurrentBluetoothConnection.OnRecived -= CurrentBluetoothConnection_OnRecived;
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
            StringBuilder tempReceiveData = new StringBuilder();

            if (model != null)
            {
                model.SetReciving();

                for (int index = 0; index < recivedEventArgs.Buffer.Length; index++)
                {
                    byte value = recivedEventArgs.Buffer.ToArray()[index];
                    byte[] valueArray = new byte[] { value };
                    model.Digit += ConvertASCIIToString(valueArray);

                    if (index == recivedEventArgs.Buffer.Length - 1)
                        tempReceiveData.Append(model.Digit);                        
                }

                CreateListOfBLEDevices(tempReceiveData);
                model.SetRecived();
            }
        }

        private string ConvertASCIIToString(byte[] asciiNumber)
        {
            Encoding ascii = Encoding.ASCII;
            return ascii.GetString(asciiNumber);
        }

        private void CreateListOfBLEDevices(StringBuilder tempReceiveData)
        {
            UpdateReceiveData(tempReceiveData);

            int amoutOfDataFromOneDevice = GetEndIndexOfBLEDeviceResponse(out int indexOfSubstring);

            if (amoutOfDataFromOneDevice <= receiveData.Length && indexOfSubstring != -1)
            {
                string device = receiveData.ToString().Substring(0, amoutOfDataFromOneDevice);
                TramsformStringToBLEDeviceInstance(device);
                numberOfReceiveCharUsed += amoutOfDataFromOneDevice;
            }
        }

        private void UpdateReceiveData(StringBuilder tempReceiveData)
        {
            receiveData.Remove(0, receiveData.Length);
            if (tempReceiveData.Length > numberOfReceiveCharUsed)
                tempReceiveData.Remove(0, numberOfReceiveCharUsed);
            receiveData.Append(tempReceiveData);
        }

        private int GetEndIndexOfBLEDeviceResponse(out int indexOfSubstring)
        {
            string endOfOneDeviceResponse = "RSSI:";
            indexOfSubstring = receiveData.ToString().IndexOf(endOfOneDeviceResponse);
            return indexOfSubstring + endOfOneDeviceResponse.Length + 4;
        }

        private void TramsformStringToBLEDeviceInstance(string fullDataFromDevice)
        {
            try
            {
                string addressPattern = "Address:";
                string namePattern = "Name:";
                string rssiPattern = "RSSI:";

                int indexOfAddressSubstring = fullDataFromDevice.IndexOf(addressPattern);
                int indexOfNameSubstring = fullDataFromDevice.IndexOf(namePattern);
                int indexOfRssiSubstring = fullDataFromDevice.IndexOf(rssiPattern);

                string address = fullDataFromDevice.Substring(indexOfAddressSubstring + addressPattern.Length,
                    indexOfNameSubstring - indexOfAddressSubstring - addressPattern.Length - 1).Trim();
                string name = fullDataFromDevice.Substring(indexOfNameSubstring + namePattern.Length,
                    indexOfRssiSubstring - indexOfNameSubstring - namePattern.Length - 1).Trim();
                string rssi = fullDataFromDevice.Substring(indexOfRssiSubstring + rssiPattern.Length + 2,
                    fullDataFromDevice.Length - indexOfRssiSubstring - rssiPattern.Length - 2);

                var device = new BLEDevice();

                var tempRssiList = new List<int>();
                tempRssiList.Add(int.Parse(rssi));

                device.Address = address;
                device.Name = name;
                device.Rssi = tempRssiList; 

                bLEDevices.Add(device);
            }
            catch (Exception e) { }           
        }
    }
}