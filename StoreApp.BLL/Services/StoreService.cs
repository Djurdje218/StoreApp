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
        public async Task AddStoreAsync(StoreDto storeDto)
        {
            var store = _mapper.Map<Store>(storeDto);
            await _storeRepository.AddStoreAsync(store);
        }

        public async Task<IEnumerable<StoreDto>> GetAllStoresAsync()
        {
            var stores = await _storeRepository.GetAllStoresAsync();
            return _mapper.Map<IEnumerable<StoreDto>>(stores);
        }

        public async Task<StoreDto> GetStoreByCodeAsync(int storeCode)
        {
            var store = await _storeRepository.GetStoreByCodeAsync(storeCode);
             return _mapper.Map<StoreDto>(store);        }
        }
}
