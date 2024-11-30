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
        string _filePath;

        public FileProductRepository(string filePath)
        {
            _filePath = filePath;
        }
        public void AddProduct(Product product)
        {
            var allProducts = GetAllProducts().ToList();

            using (var sw = new StreamWriter(_filePath, true))
            {
                sw.WriteLine($"{product.Name},{product.StoreCode},{product.Quantity},{product.Price}");
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            var products = new List<Product>();

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var data = line.Split(',');
                    if (data.Length != 4)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                    products.Add(new Product
                    {
                        Name = data[0],
                        StoreCode = int.Parse(data[1]),
                        Quantity = int.Parse(data[2]),
                        Price = decimal.Parse(data[3])
                    });
                }
            }

            return products;
        }

        public IEnumerable<Product> GetProductsByStore(int storeCode)
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            var products = new List<Product>();

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
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

        public void UpdateProduct(Product product)
        {
            var lines = File.ReadAllLines(_filePath).ToList();
            var updated = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var fields = lines[i].Split(',');
                if (fields[0] == product.Name) // Assuming `Id` is the first column
                {
                    // Update the product details in the CSV row
                    fields[0] = product.Name;          // Name
                    fields[1] = product.StoreCode.ToString(); // StoreCode
                    fields[2] = product.Quantity.ToString();  // Quantity
                    fields[3] = product.Price.ToString("F2"); // Price
                    lines[i] = string.Join(",", fields); // Rebuild the CSV row
                    updated = true;
                    break;
                }
            }

            if (updated)
            {
                File.WriteAllLines(_filePath, lines); // Overwrite the file with updated data
            }
            else
            {
                throw new Exception($"Product with Id {product.Name} not found.");
            }
        }
    }
}
