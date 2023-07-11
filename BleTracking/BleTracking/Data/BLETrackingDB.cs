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
            db.CreateTableAsync<RssiModel>().Wait();
        }

        public Task<List<DeviceModel>> GetDevicesAsync()
        {           
            // получения списка с записями
            return db.Table<DeviceModel>().ToListAsync();
        }

        public Task<DeviceModel> GetDevicesAsync(int id)
        {
            return db.Table<DeviceModel>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<DeviceModel> GetDevicesAsync(string address)
        {
            return db.Table<DeviceModel>()
                .Where(i => i.Address == address)
                .FirstOrDefaultAsync();
        }

        public Task<List<RssiModel>> GetRssiValuesAsync()
        {
            return db.Table<RssiModel>().ToListAsync();
        }

        public Task<RssiModel> GetRssiValuesAsync(int id)
        {
            return db.Table<RssiModel>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveDataAsync(Model device)
        {
            if (device.Id != 0)
            {
                return db.UpdateAsync(device);
            }
            else
            {
                return db.InsertAsync(device);
            }
        }

        public Task<int> DeteleDataAsync(Model device)
        {
            return db.DeleteAsync(device);
        }
    }
}
