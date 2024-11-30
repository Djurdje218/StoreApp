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
    public class StoreService : IStoreService
    {
        private IStoreRepository _storeRepository;
        private IMapper          _mapper;

        public StoreService(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public void AddStore(StoreDto storeDto)
        {
            var store = _mapper.Map<Store>(storeDto);
            _storeRepository.AddStore(store);
        }

        public IEnumerable<StoreDto> GetAllStores()
        {
            var stores = _storeRepository.GetAllStores();
            return _mapper.Map<IEnumerable<StoreDto>>(stores);
        }

        public StoreDto GetStoreByCode(int storeCode)
        {
        var store = _storeRepository.GetStoreByCode(storeCode);
        return _mapper.Map<StoreDto>(store);        }
    }
}
