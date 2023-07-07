﻿using BleTracking.ESP32Data;
using BleTracking.Models;
using BleTracking.ViewModel;
using Plugin.BluetoothClassic.Abstractions;
using SQLite;
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
        DeviceModel deviceModel = new DeviceModel();
        RssiModel rssiModel = new RssiModel();
        List<RssiModel> rssiList = new List<RssiModel>();

        public TerminalPage()
        {
            InitializeComponent();

            if (App.CurrentBluetoothConnection != null)
            {
                App.CurrentBluetoothConnection.OnStateChanged += CurrentBluetoothConnection_OnStateChanged;
                App.CurrentBluetoothConnection.OnRecived += CurrentBluetoothConnection_OnRecived;
            }
        }

        protected override async void OnAppearing()
        {
            collectionView.ItemsSource = await App.BLETrackingDB.GetDevicesAsync();

            base.OnAppearing();
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

        public void CurrentBluetoothConnection_OnRecived(object sender, Plugin.BluetoothClassic.Abstractions.RecivedEventArgs recivedEventArgs)
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
                var newDevice = TramsformStringToBLEDeviceInstance(device);
                //AddNewBLEDevice(newDevice);
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
                //bLEDevices.Add(device);
            }
            catch (Exception e) {
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
            AddNewRssiToDB(rssi, device.Id);

            device.Distance = Math.Round(ConvertRssiToDistance(-69, rssi, 3), 2);

            rssiList.Add(rssiModel);
            device.RssiValues = rssiList;
            

            Task.Run(() => SaveDeviceToDB(device));            
        }

        private void AddNewDeviceToDB(BLEDevice device)
        {
            rssiModel.ReceivedRssi = device.Rssi[0];

            deviceModel.Address = device.Address;
            deviceModel.Name = device.Name;
            deviceModel.Distance = Math.Round(ConvertRssiToDistance(-69, rssiModel.FilteredRssi, 3), 2);

            Task.Run(() => SaveDeviceToDB(deviceModel));
        }

        private void AddNewRssiToDB(int rssi, int id)
        {
            rssiModel.ReceivedRssi = rssi;
            rssiModel.FilteredRssi = KalmanFilter(rssi, 0.25);
            rssiModel.DateTime = DateTime.Now;
            rssiModel.DeviceId = id;
            Task.Run(() => SaveRssiToDB(rssiModel));
        }

        private void AddNewBLEDevice(BLEDevice device)
        {
            int receivedRssi;

            if (bLEDevices.Count > 0)
            {
                foreach (var item in bLEDevices)
                {
                    if (device.Address == item.Address)
                    {
                        rssiModel.ReceivedRssi = device.Rssi[0];
                        rssiModel.FilteredRssi = KalmanFilter(device.Rssi[0], 0.25);
                        rssiModel.DateTime = DateTime.Now;

                        var list = App.BLETrackingDB.GetDevicesAsync();

                        List<DeviceModel> devices = new List<DeviceModel>();
                        devices.AddRange(list.Result);

                        Task.Run(() => SaveRssiToDB(rssiModel));

                        item.Rssi.Add(device.Rssi[0]);
                        return;
                    }
                }
            }

            rssiModel.ReceivedRssi = device.Rssi[0];
            rssiModel.FilteredRssi = KalmanFilter(device.Rssi[0], 0.25);
            rssiModel.DateTime = DateTime.Now;
            rssiModel.DeviceId = deviceModel.Id;

            deviceModel.Address = device.Address;
            deviceModel.Name = device.Name;
            deviceModel.Distance = Math.Round(ConvertRssiToDistance(-69, rssiModel.FilteredRssi, 3), 2);
            //deviceModel.RssiValues.Add(rssiModel);


            bLEDevices.Add(device);
            Task.Run(() => SaveRssiToDB(rssiModel));

            Task.Run(() => SaveDeviceToDB(deviceModel));
        }


        double Q = 0.05; // скорость реакции на изменение (подобрать вручную)
        double P0 = 0.0;
        double Pk = 1.0;
        double X0 = 0.0;
        double X = 0.0;
        double F = 1.0;
        double H = 1.0;
        private double KalmanFilter(int rssi, double R)
        {
            X0 = F * X;
            P0 = F * Pk * F + Q;

            double K = H * P0 / (H * P0 * H + R);
            X = X0 + K * (rssi - H * X0);
            Pk = (1 - K * H) * P0;
            return X;
        }

        private double ConvertRssiToDistance(int measuredPower, double rssi, int n)
        {
            double dist = Math.Pow(10.0, ((measuredPower + rssi) / (10 * n)));
            return dist;
        }

        private async Task SaveDeviceToDB(DeviceModel model)
        {
            //DeviceModel model = new DeviceModel() { Address = "3", Name = "ddsd", PositonId = 32 };
            //DeviceModel note = (DeviceModel)BindingContext;
            //note.Date = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                await App.BLETrackingDB.SaveDataAsync(model);
            }

            //await Navigation.PushAsync(new TerminalPage());
            await Shell.Current.GoToAsync("..");
            //await Shell.Current.GoToAsync("//TerminalPage");
            //await Navigation.PushAsync(new BLETrackingPage());

        }

        private async Task SaveRssiToDB(RssiModel model)
        {
            //DeviceModel model = new DeviceModel() { Address = "3", Name = "ddsd", PositonId = 32 };
            //DeviceModel note = (DeviceModel)BindingContext;
            //note.Date = DateTime.Now;

            await App.BLETrackingDB.SaveDataAsync(model);
            await Shell.Current.GoToAsync("..");
            //await Shell.Current.GoToAsync("//TerminalPage");
            //await Navigation.PushAsync(new BLETrackingPage());

        }

        private async void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                DeviceModel model = e.CurrentSelection.FirstOrDefault() as DeviceModel;
                await App.BLETrackingDB.DeteleDataAsync(model);

                //await Navigation.PushAsync(new TerminalPage());
                //await Shell.Current.GoToAsync(nameof(TerminalPage));
            }
        }
    }
}