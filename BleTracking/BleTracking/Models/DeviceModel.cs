using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace BleTracking.Models
{
    [Table("Device")]
    public class DeviceModel : Model
    {
        public string Address { get; set; }
        public string Name { get; set; }  
        public double Distance { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<RssiModel> RssiValues { get; set; }
    }
}
