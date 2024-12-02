using DAL.Infrastructure;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class FileStoreRepository : IStoreRepository
    {
        private string _filePath;

        public FileStoreRepository(string filePath)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", filePath);
        }

        public async Task AddStoreAsync(Store store)
        {
            var allStores = await GetAllStoresAsync();

            if (allStores.Any(s => s.Code == store.Code))
            {
                throw new ArgumentException($"A store with code {store.Code} already exists.");
            }

            // Asynchronously write the store data to the file
            using (var sw = new StreamWriter(_filePath, true))
            {
                await sw.WriteLineAsync($"{store.Code},{store.Name},{store.Address}");
            }
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            var stores = new List<Store>();

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var data = line.Split(',');
                    if (data.Length != 3)
                    {
                        continue;
                    }

                    stores.Add(new Store
                    {
                        Code = int.Parse(data[0]),
                        Name = data[1],
                        Address = data[2]
                    });
                }
            }

            return stores;
        }

        public async Task<Store> GetStoreByCodeAsync(int code)
        {
            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var data = line.Split(",");
                    if (int.Parse(data[0]) == code)
                    {
                        return new Store
                        {
                            Code = int.Parse(data[0]),
                            Name = data[1],
                            Address = data[2]
                        };
                    }
                }
            }
            return null;
        }
    }
}
