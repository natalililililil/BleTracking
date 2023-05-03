using BleTracking.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BleTracking.Data
{
    public class BLETrackingDB
    {
        readonly SQLiteAsyncConnection db;

        public BLETrackingDB(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);

            db.CreateTableAsync<DeviceModel>().Wait();
        }

        public Task<List<DeviceModel>> GetDevicesAsync()
        {
            // получения списка с записями
            return db.Table<DeviceModel>().ToListAsync();
        }

        public Task<DeviceModel> GetDevicesAsync(int id)
        {
            // получение конкретной записки
            return db.Table<DeviceModel>()
                .Where(i => i.DeviceId == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveDevicesAcync(DeviceModel device)
        {
            if (device.DeviceId != 0)
            {
                return db.UpdateAsync(device);
            }
            else
            {
                return db.InsertAsync(device);
            }
        }

        public Task<int> DeteleDeviceAsync(DeviceModel device)
        {
            return db.DeleteAsync(device);
        }
    }
}
