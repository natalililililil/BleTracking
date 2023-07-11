using BleTracking.ESP32Data;
using BleTracking.Filter;
using BleTracking.Models;
using BleTracking.ViewModel;
using Plugin.BluetoothClassic.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BleTracking.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentDevicePage : ContentPage 
    {
        StringBuilder receiveData = new StringBuilder();
        int numberOfReceiveCharUsed;
        DeviceModel deviceModel = new DeviceModel();
        RssiModel rssiModel = new RssiModel();
        List<RssiModel> rssiList = new List<RssiModel>();
        StringBuilder tempModelDistance = new StringBuilder();
        private string Address { get; set; }

        public CurrentDevicePage(string address, int id)
        {
            InitializeComponent();

            DistanceViewModel distanceViewModel = (DistanceViewModel)BindingContext;
            distanceViewModel.PropertyChanged += Model_PropertyChanged;

            if (App.CurrentBluetoothConnection != null)
                App.CurrentBluetoothConnection.OnRecived += CurrentBluetoothConnection_OnRecived;

            Address = address;
        }

        public void CurrentBluetoothConnection_OnRecived(object sender, Plugin.BluetoothClassic.Abstractions.RecivedEventArgs recivedEventArgs)
        {
            DistanceViewModel model = (DistanceViewModel)BindingContext;
            StringBuilder tempReceiveData = new StringBuilder();

            if (model != null)
            {
                model.SetReciving();
                for (int index = 0; index < recivedEventArgs.Buffer.Length; index++)
                {
                    byte value = recivedEventArgs.Buffer.ToArray()[index];
                    byte[] valueArray = new byte[] { value };
                    tempModelDistance.Append(ConvertASCIIToString(valueArray));

                    if (index == recivedEventArgs.Buffer.Length - 1)
                        tempReceiveData.Append(tempModelDistance);
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

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DistanceViewModel.Properties.Distance.ToString())
            {
                TransmitCurrentDigit();
            }
        }
        private void TransmitCurrentDigit()
        {
            DistanceViewModel model = (DistanceViewModel)BindingContext;
            if (model != null && !model.Reciving)
            {

            }
        }

        private void CreateListOfBLEDevices(StringBuilder tempReceiveData)
        {
            
            UpdateReceiveData(tempReceiveData);

            int amoutOfDataFromOneDevice = GetEndIndexOfBLEDeviceResponse(out int indexOfSubstring);

            if (amoutOfDataFromOneDevice <= receiveData.Length && indexOfSubstring != -1)
            {
                string device = receiveData.ToString().Substring(0, amoutOfDataFromOneDevice);
                var newDevice = TramsformStringToBLEDeviceInstance(device);

                if (newDevice.Address == Address)
                    Add(newDevice);

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

        private BLEDevice TramsformStringToBLEDeviceInstance(string fullDataFromDevice)
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

                return device;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Add(BLEDevice device)
        {
            //считать данные
            var tempList = App.BLETrackingDB.GetDevicesAsync();
            List<DeviceModel> devices = new List<DeviceModel>();
            devices.AddRange(tempList.Result);

            if (devices.Count > 0)
            {
                foreach (var item in devices)
                {
                    if (device.Address == item.Address)
                    {
                        UpdateDeviceAndRssiData(item, device.Rssi[0]);
                        return;
                    }
                }
            }

            AddNewDeviceToDB(device);

            AddNewRssiToDB(device.Rssi[0], devices[0].Id);
        }

        private void UpdateDeviceAndRssiData(DeviceModel device, int rssi)
        {
            DistanceViewModel model = (DistanceViewModel)BindingContext;
            AddNewRssiToDB(rssi, device.Id);

            device.Distance = Math.Round(ConvertRssiToDistance(-69, rssi, 3), 2);
            model.Distance = device.Distance.ToString() + "м";

            rssiList.Add(rssiModel);
            device.RssiValues = rssiList;


            Task.Run(() => SaveDeviceToDB(device));
        }

        private void AddNewDeviceToDB(BLEDevice device)
        {
            rssiModel.ReceivedRssi = device.Rssi[0];

            deviceModel.Address = device.Address;
            deviceModel.Name = device.Name;
            deviceModel.Distance = (int)ConvertRssiToDistance(-69, rssiModel.FilteredRssi, 3);

            Task.Run(() => SaveDeviceToDB(deviceModel));
        }

        private void AddNewRssiToDB(int rssi, int id)
        {
            rssiModel.ReceivedRssi = rssi;
            rssiModel.FilteredRssi = KalmalFilter.GetFileterData(rssi, 0.25);
            rssiModel.DateTime = DateTime.Now;
            rssiModel.DeviceId = id;
            Task.Run(() => SaveRssiToDB(rssiModel));
        }

        private double ConvertRssiToDistance(int measuredPower, double rssi, int n)
        {
            double dist = Math.Pow(10.0, ((measuredPower + rssi) / (10 * n)));
            return dist;
        }

        private async Task SaveDeviceToDB(DeviceModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                await App.BLETrackingDB.SaveDataAsync(model);
            }
        }

        private async Task SaveRssiToDB(RssiModel model)
        {
            await App.BLETrackingDB.SaveDataAsync(model);
        }
    }
}