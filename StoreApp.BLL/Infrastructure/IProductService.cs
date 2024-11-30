using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IProductService
    {
        void AddProduct(ProductDto productDto);
        IEnumerable<ProductDto> GetAllProducts();
        IEnumerable<ProductDto> GetProductsByStore(int storeCode);
        ProductDto FindCheapestStoreForProduct(string productName);
        decimal PurchaseProducts(int storeCode, Dictionary<string, int> products);
        (int storeCode, decimal totalCost) FindCheapestStoreForBatch(Dictionary<string, int> productBatch);
        void RestockProducts(int storeCode, Dictionary<string, (int quantity, decimal price)> productUpdates);
        IEnumerable<ProductDto> FindPurchasableProducts(int storeCode, decimal budget);

    }
}
