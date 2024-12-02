using static System.Formats.Asn1.AsnWriter;
using DAL.Entities;
namespace DAL.Infrastructure
{
    public interface IStoreRepository
    {
        Task AddStoreAsync(Store store);
        Task<Store> GetStoreByCodeAsync(int code);
        Task<IEnumerable<Store>> GetAllStoresAsync();
    }
}
