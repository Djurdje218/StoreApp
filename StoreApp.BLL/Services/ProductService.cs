using AutoMapper;
using BLL.DTO;
using DAL.Entities;
using DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void AddProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _productRepository.AddProduct(product);
        }

        public (int storeCode, decimal totalCost) FindCheapestStoreForBatch(Dictionary<string, int> productBatch)
        {
            var stores = _storeRepository.GetAllStores();
            decimal minCost = decimal.MaxValue;
            int bestStore = -1;

            foreach (var store in stores)
            {
                var storeProducts = _productRepository.GetProductsByStore(store.Code);
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

        public ProductDto FindCheapestStoreForProduct(string productName)
        {
            var products = _productRepository.GetAllProducts()
                .Where(p => p.Name == productName)
                .OrderBy(p => p.Price)
                .FirstOrDefault();

            return _mapper.Map<ProductDto>(products);
        }

        public IEnumerable<ProductDto> FindPurchasableProducts(int storeCode, decimal budget)
        {
            var products = _productRepository.GetProductsByStore(storeCode)
                .Where(p => p.Price > 0 && budget >= p.Price)
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    StoreCode = p.StoreCode,
                    Quantity = (int)(budget / p.Price),
                    Price = p.Price
                });

            return products;
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            var products = _productRepository.GetAllProducts();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public IEnumerable<ProductDto> GetProductsByStore(int storeCode)
        {
            var products = _productRepository.GetProductsByStore(storeCode);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public decimal PurchaseProducts(int storeCode, Dictionary<string, int> products)
        {
            var storeProducts = _productRepository.GetProductsByStore(storeCode).ToList();
            decimal totalCost = 0;

            foreach (var (productName, quantity) in products)
            {
                var product = storeProducts.FirstOrDefault(p => p.Name == productName);

                if (product == null || product.Quantity < quantity)
                    throw new InvalidOperationException($"Cannot purchase {quantity} of {productName}");

                totalCost += product.Price * quantity;
                product.Quantity -= quantity;
            }

            return totalCost;
        }

        public void RestockProducts(int storeCode, Dictionary<string, (int quantity, decimal price)> productUpdates)
        {
            foreach (var (productName, (quantity, price)) in productUpdates)
            {
                var product = _productRepository.GetProductsByStore(storeCode).FirstOrDefault(p => p.Name == productName);

                if (product == null)
                {
                    Console.WriteLine($"Product not found. Adding new product: {productName}");
                    _productRepository.AddProduct(new Product
                    {
                        Name = productName,
                        StoreCode = storeCode,
                        Quantity = quantity,
                        Price = price
                    });
                }
                else
                {
                    product.Quantity += quantity;
                    product.Price = price;
                    _productRepository.UpdateProduct(product); // save  to the CSV
                }
            }
        }
    }
}
