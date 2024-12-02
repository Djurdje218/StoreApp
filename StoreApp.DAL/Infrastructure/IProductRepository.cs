using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Infrastructure
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<IEnumerable<Product>> GetProductsByStoreAsync(int storeCode);
        Task<IEnumerable<Product>> GetAllProductsAsync();


    }
}
