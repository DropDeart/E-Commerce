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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly DbSet<Category> _dbSet;

        public CategoryRepository(AppDbContext context)
        {
            _appDbContext = context;
            _dbSet = _appDbContext.Set<Category>();
        }
        public IQueryable<Category> GetAll(string? text = null, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(text) ||
                    x.Description.ToLower().Contains(text) ||
                    x.Id.ToString().Contains(text));
            }

            return query;
        }

        public IQueryable<Category> GetWhere(Expression<Func<Category, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public async Task<Category?> GetSingleAsync(Expression<Func<Category, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(Category model)
        {
            await _dbSet.AddAsync(model);
            return await SaveAsync();
        }

        public async Task<bool> AddRangeAsync(List<Category> models)
        {
            await _dbSet.AddRangeAsync(models);
            return await SaveAsync();
        }

        public bool Remove(Category model)
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

        public bool RemoveRange(List<Category> models)
        {
            _dbSet.RemoveRange(models);
            return SaveAsync().Result;
        }

        public bool Update(Category model)
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
