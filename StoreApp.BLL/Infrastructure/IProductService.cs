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
        Task AddProductAsync(ProductDto productDto);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsByStore(int storeCode);
        Task<ProductDto> FindCheapestStoreForProductAsync(string productName);
        Task<decimal> PurchaseProductsAsync(int storeCode, Dictionary<string, int> products);
        Task<(int storeCode, decimal totalCost)> FindCheapestStoreForBatchAsync(Dictionary<string, int> productBatch);
        Task RestockProductsAsync(int storeCode, Dictionary<string, (int quantity, decimal price)> productUpdates);
        Task<IEnumerable<ProductDto>> FindPurchasableProductsAsync(int storeCode, decimal budget);

    }
}
