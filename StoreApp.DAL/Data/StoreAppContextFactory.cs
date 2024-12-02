using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class StoreAppContextFactory : IDesignTimeDbContextFactory<StoreAppContext>
    {
        public StoreAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreAppContext>();
            optionsBuilder.UseSqlite("Data Source=Data/StoreApp.db");

            return new StoreAppContext(optionsBuilder.Options);
        }
    }
}