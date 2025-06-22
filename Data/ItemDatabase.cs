using MauiBench.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBench.Data
{
    public class ItemDatabase
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<ItemDatabase> Instance =
            new AsyncLazy<ItemDatabase>(async () =>
            {
                var instance = new ItemDatabase();
                try
                {
                    if (Database == null)
                    {
                        throw new InvalidOperationException("Database connection is not initialized.");
                    }

                    CreateTableResult result = await Database.CreateTableAsync<BenchmarkModel>();
                }
                catch (Exception)
                {
                    throw;
                }
                return instance;
            });

        public ItemDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<BenchmarkModel>> GetItemsAysnc()
        {
            return Database.Table<BenchmarkModel>().ToListAsync();
        }

        public Task<int> DeleteAllItemsAsync()
        {
            return Database.DeleteAllAsync<BenchmarkModel>();
        }

        public Task<int> SaveItemAsync(BenchmarkModel item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }
    }
}