using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using SQLiteNetExtensions.Attributes;

namespace BleTracking.Models
{
    [Table("Rssi")]
    public class RssiModel
    {
        [PrimaryKey, AutoIncrement]
        public int RssiId { get; set; }
        public int ReceivedRssi { get; set; }
        public int FilteredRssi { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey(typeof(DeviceModel))]
        public int DeviceId { get; set; }
    }
}
