using AutoMapper;
using BLL.DTO;
using DAL.Entities;
using DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;



namespace BLL.Services
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IStoreRepository storeRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public async Task AddProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddProductAsync(product);
        }

        public async Task <(int storeCode, decimal totalCost)> FindCheapestStoreForBatchAsync(Dictionary<string, int> productBatch)
        {
            var stores = await _storeRepository.GetAllStoresAsync();
            decimal minCost = decimal.MaxValue;
            int bestStore = -1;

            foreach (var store in stores)
            {
                var storeProducts = await _productRepository.GetProductsByStoreAsync(store.Code);
                decimal totalCost = 0;
                bool canPurchase = true;

                foreach (var (productName, quantity) in productBatch)
                {
                    var product = storeProducts.FirstOrDefault(p => p.Name == productName);
                    if (product == null || product.Quantity < quantity)
                    {
                        canPurchase = false;
                        break;
                    }

                    totalCost += product.Price * quantity;
                }

                if (canPurchase && totalCost < minCost)
                {
                    minCost = totalCost;
                    bestStore = store.Code;
                }
            }

            return (bestStore, minCost);
        }

        public async Task<ProductDto> FindCheapestStoreForProductAsync(string productName)
        {
            var products = await _productRepository.GetAllProductsAsync();  // Await the async call

            var cheapestProduct = products
                .Where(p => p.Name == productName)
                .OrderBy(p => p.Price)
                .FirstOrDefault();

            return _mapper.Map<ProductDto>(cheapestProduct);  // Map the result to ProductDto
        }

        public async Task<IEnumerable<ProductDto>> FindPurchasableProductsAsync(int storeCode, decimal budget)
        {
            var products = await _productRepository.GetProductsByStoreAsync(storeCode);  // Make sure GetProductsByStoreAsync is async

            var purchasableProducts = products
                .Where(p => p.Price > 0 && budget >= p.Price)
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    StoreCode = p.StoreCode,
                    Quantity = (int)(budget / p.Price),
                    Price = p.Price
                });

            return purchasableProducts;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByStore(int storeCode)
        {
            var products = await _productRepository.GetProductsByStoreAsync(storeCode);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<decimal> PurchaseProductsAsync(int storeCode, Dictionary<string, int> products)
        {
            var storeProducts = await _productRepository.GetProductsByStoreAsync(storeCode);  // Await the async repository call
            decimal totalCost = 0;

            foreach (var (productName, quantity) in products)
            {
                var product = storeProducts.FirstOrDefault(p => p.Name == productName);

                if (product == null || product.Quantity < quantity)
                    throw new InvalidOperationException($"Cannot purchase {quantity} of {productName}");

                totalCost += product.Price * quantity;
                product.Quantity -= quantity;

                // Optionally update the product in the repository after purchase
                await _productRepository.UpdateProductAsync(product);  // Ensure this is also async
            }

            return totalCost;
        }

        public async Task RestockProductsAsync(int storeCode, Dictionary<string, (int quantity, decimal price)> productUpdates)
        {
            foreach (var (productName, (quantity, price)) in productUpdates)
            {
                // First, await the async call to get the products by store
                var storeProducts = await _productRepository.GetProductsByStoreAsync(storeCode);

                var product = storeProducts.FirstOrDefault(p => p.Name == productName);

                if (product == null)
                {
                    Console.WriteLine($"Product not found. Adding new product: {productName}");

                    // If the product is not found, add it asynchronously
                    await _productRepository.AddProductAsync(new Product
                    {
                        Name = productName,
                        StoreCode = storeCode,
                        Quantity = quantity,
                        Price = price
                    });
                }
                else
                {
                    // If the product is found, update its quantity and price
                    product.Quantity += quantity;
                    product.Price = price;

                    // Ensure the update is performed asynchronously
                    await _productRepository.UpdateProductAsync(product); // await the async update
                }
            }
        }

    }
}
