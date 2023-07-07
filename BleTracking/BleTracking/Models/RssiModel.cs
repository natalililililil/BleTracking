using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using SQLiteNetExtensions.Attributes;

namespace BleTracking.Models
{
    [Table("Rssi")]
    public class RssiModel : Model
    {
        public int ReceivedRssi { get; set; }
        public double FilteredRssi { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey(typeof(DeviceModel))]
        public int DeviceId { get; set; }
    }
}
