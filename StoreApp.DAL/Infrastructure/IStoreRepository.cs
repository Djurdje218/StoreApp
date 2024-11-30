using static System.Formats.Asn1.AsnWriter;
using DAL.Entities;
namespace DAL.Infrastructure
{
    public interface IStoreRepository
    {
        void AddStore(Store store);
        Store GetStoreByCode(int code);
        IEnumerable<Store> GetAllStores();
    }
}
