using Ljuss.Core.Entities;
using Ljuss.Core.Repositories;
using LjussAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LJussAPI.Persistance.Repositories
{
    public class ShippingAddressRepository : IShippingAddressRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly DbSet<ShippingAddress> _dbSet;

        public ShippingAddressRepository(AppDbContext context)
        {
            _appDbContext = context;
            _dbSet = _appDbContext.Set<ShippingAddress>();
        }
        public IQueryable<ShippingAddress> GetAll(string? text = null, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToLower();
                query = query.Where(x =>
                    x.Title.ToLower().Contains(text) ||
                    x.City.Name.ToLower().Contains(text) ||
                    x.AddressLine1.ToLower().Contains(text) ||
                    x.Id.ToString().Contains(text));
            }

            return query;
        }

        public IQueryable<ShippingAddress> GetWhere(Expression<Func<ShippingAddress, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public async Task<ShippingAddress?> GetSingleAsync(Expression<Func<ShippingAddress, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ShippingAddress?> GetByIdAsync(Guid id, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(ShippingAddress model)
        {
            await _dbSet.AddAsync(model);
            return await SaveAsync();
        }

        public async Task<bool> AddRangeAsync(List<ShippingAddress> models)
        {
            await _dbSet.AddRangeAsync(models);
            return await SaveAsync();
        }

        public bool Remove(ShippingAddress model)
        {
            _dbSet.Remove(model);
            return SaveAsync().Result;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            return await SaveAsync();
        }

        public bool RemoveRange(List<ShippingAddress> models)
        {
            _dbSet.RemoveRange(models);
            return SaveAsync().Result;
        }

        public bool Update(ShippingAddress model)
        {
            _dbSet.Update(model);
            return SaveAsync().Result;
        }

        public async Task<bool> SaveAsync()
        {
            return await _appDbContext.SaveChangesAsync() > 0;
        }
    }
}
