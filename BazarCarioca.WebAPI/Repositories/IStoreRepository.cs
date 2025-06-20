﻿using BazarCarioca.WebAPI.DTOs;
using BazarCarioca.WebAPI.Models;

namespace BazarCarioca.WebAPI.Repositories
{
    public interface IStoreRepository : IRepository<Store>, IImageRepository<Store, StoreUpdateDTO>
    {
        Task<IEnumerable<Store>> GetByShopkeeperIdAsync(int Id);
    }
}
