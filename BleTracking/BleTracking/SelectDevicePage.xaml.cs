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
    public partial class SelectDevicePage : ContentPage
    {
        public SelectDevicePage()
        {
            InitializeComponent();
            FillBondedDevices();
        }

        private void FillBondedDevices()
        {
            var adapter = DependencyService.Resolve<IBluetoothAdapter>();
            lvBondedDevices.ItemsSource = adapter.BondedDevices;
        }

        private async void lvBondedDevices_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var device = (BluetoothDeviceModel)e.SelectedItem;
            if (device != null)
            {
                await Navigation.PushAsync(new TransiverPage { BindingContext = device});
            }

            //убрать выделение с текущего элемента
            lvBondedDevices.SelectedItem = null;
        }
    }
}