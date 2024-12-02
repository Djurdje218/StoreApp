using DAL.Entities;
using DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace DAL.Repositories
{
    public class FileProductRepository : IProductRepository
    {
        private string _filePath;

        public FileProductRepository(string filePath)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", filePath);
        }
        public async Task AddProductAsync(Product product)
        {
            var allProducts = await GetAllProductsAsync();

            using (var sw = new StreamWriter(_filePath, true))
            {
              await  sw.WriteLineAsync($"{product.id},{product.Name},{product.StoreCode},{product.Quantity},{product.Price}");
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            var products = new List<Product>();

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var data = line.Split(',');
                    if (data.Length != 5)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                     products.Add(new Product
                    {
                        id = int.Parse(data[0]),
                        Name = data[1],
                        StoreCode = int.Parse(data[2]),
                        Quantity = int.Parse(data[3]),
                        Price = decimal.Parse(data[4])
                    });
                }
            }

            return products;
        }

        public async  Task<IEnumerable<Product>> GetProductsByStoreAsync(int storeCode)
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            var products = new List<Product>();

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var data = line.Split(",");
                    if (data.Length != 4)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                    if (int.Parse(data[1]) == storeCode)
                    {
                        products.Add(new Product
                        {
                            Name = data[0],
                            StoreCode = int.Parse(data[1]),
                            Quantity = int.Parse(data[2]),
                            Price = decimal.Parse(data[3])
                        });


                    }

                }
            }
            return products;
        }

        public async Task UpdateProductAsync(Product product)
        {
            var lines = (await File.ReadAllLinesAsync(_filePath)).ToList();
            var updated = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var fields = lines[i].Split(',');
                if (fields[0] == product.Name) 
                {
                    // Update the product details in the CSV row
                    fields[0] = product.id.ToString();          
                    fields[1] = product.Name;         
                    fields[2] = product.StoreCode.ToString(); 
                    fields[3] = product.Quantity.ToString(); 
                    fields[4] = product.Price.ToString("F2"); 
                    lines[i] = string.Join(",", fields); 
                    updated = true;
                    break;
                }
            }

            if (updated)
            {
                await File.WriteAllLinesAsync(_filePath, lines); // Overwrite  file
            }
            else
            {
                throw new Exception($"Product with Id {product.Name} not found.");
            }
        }
    }
}
