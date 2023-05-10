using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BleTracking.Models
{
    public class Model
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
