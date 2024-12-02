using DAL.Entities;
using DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DatabaseProductRepository : IProductRepository
    {
        private readonly StoreAppContext _context;

        public DatabaseProductRepository(StoreAppContext context)
        {
            _context = context;
        }

        public async Task AddProductAsync(Product product)
        {
            if(!_context.Stores.Any(s => s.Code == product.StoreCode))
            {
                throw new InvalidOperationException($"Store with Code {product.StoreCode} does not exist.");

            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByStoreAsync(int storeCode)
        {
            return await _context.Products.Where(p => p.StoreCode == storeCode).ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.id);
            if (existing == null)
            {
                throw new Exception($"Product with Id {product.id} not found.");
            }
            existing.Name = product.Name;
            existing.Quantity = product.Quantity;
            existing.Price = product.Price;
            existing.StoreCode = product.StoreCode;

            _context.SaveChanges();
        }
    }
}