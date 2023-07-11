using System.Collections.Generic;

namespace BleTracking.ESP32Data
{
    internal class BLEDevice
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public List<int> Rssi { get; set; }
    }
}
