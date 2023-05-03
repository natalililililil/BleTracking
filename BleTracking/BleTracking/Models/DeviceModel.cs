using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace BleTracking.Models
{
    [Table("Device")]
    public class DeviceModel
    {
        [PrimaryKey, AutoIncrement, Column("DeviceId")]
        public int DeviceId { get; set; }

        [Column("Address")]
        public string Address { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Distance")]     
        public int Distance { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<RssiModel> RssiValues { get; set; }
    }
}
