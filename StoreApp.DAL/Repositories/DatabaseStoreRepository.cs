using DAL.Infrastructure;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using static System.Formats.Asn1.AsnWriter;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DatabaseStoreRepository : IStoreRepository
    {
        private readonly StoreAppContext _context;

        public DatabaseStoreRepository(StoreAppContext context)
        {
            _context = context;
        }

        public async Task AddStoreAsync(Store store)
        {
            if (_context.Stores.Any(s => s.Code == store.Code))
            {
                throw new InvalidOperationException($"Store with Code {store.Code} already exists.");

            }
            await _context.Stores.AddAsync(store);
            await _context.SaveChangesAsync();

   
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _context.Stores.ToListAsync();
        }

        public async Task<Store> GetStoreByCodeAsync(int storeCode)
        {
            var store = await _context.Stores.FindAsync(storeCode);

            if (store == null)
            {
                throw new InvalidOperationException($"Store with Code {storeCode} does not exist.");
            }

            return store;
        }
    }
}