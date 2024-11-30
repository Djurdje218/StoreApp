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
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        IEnumerable<Product> GetProductsByStore(int storeCode);
        IEnumerable<Product> GetAllProducts();


    }
}
