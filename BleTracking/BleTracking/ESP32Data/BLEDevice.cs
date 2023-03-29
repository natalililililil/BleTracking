using System;
using System.Collections.Generic;
using System.Text;

namespace BleTracking.ESP32Data
{
    internal class BLEDevice
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public int[] Rssi { get; set; }
        //public int[] Distance { get; set; }
    }
}
