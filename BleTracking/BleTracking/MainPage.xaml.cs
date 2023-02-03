using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BleTracking
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            List<int> f = new List<int>() { 1, 2};
            Console.WriteLine(f.Where(x => x>2));
            InitializeComponent();
        }
    }
}
