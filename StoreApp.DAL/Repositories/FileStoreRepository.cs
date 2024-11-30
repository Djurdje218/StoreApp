using DAL.Infrastructure;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class FileStoreRepository : IStoreRepository
    {
        string _filePath;

        public FileStoreRepository(string filePath)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", filePath);
        }
        public void AddStore(Store store)
        {
            var allStores = GetAllStores().ToList();

            if (allStores.Any(s => s.Code == store.Code))
            {
                throw new ArgumentException($"A store with code {store.Code} already exists.");
            }
            //store.Code = allStores.Any() ? allStores.Max(s => s.Code) + 1 : 1;  // If code already exists do Code + 1 and save it

            using (var sw = new StreamWriter(_filePath, true))
            {
                sw.WriteLine($"{store.Code},{store.Name},{store.Address}");
            }
        }
        public IEnumerable<Store> GetAllStores()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            var stores = new List<Store>();

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var data = line.Split(',');
                    if (data.Length != 3)
                    {
                        continue;
                    }

                    stores.Add(new Store { Code = int.Parse(data[0]), Name = data[1], Address = data[2] });
                }
            }

            return stores;
        }

        public Store GetStoreByCode(int code)
        {
            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
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
