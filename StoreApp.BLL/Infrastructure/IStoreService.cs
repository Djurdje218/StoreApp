using BLL.DTO;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace BLL.Services
{
    public interface IStoreService
    {
        void AddStore(StoreDto storeDto);
        IEnumerable<StoreDto> GetAllStores();
        StoreDto GetStoreByCode(int storeCode);
    }
}
