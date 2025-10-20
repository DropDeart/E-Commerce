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
using Attribute = Ljuss.Core.Entities.Attribute;

namespace LJussAPI.Persistance.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Attribute> _dbSet;

        public AttributeRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Attribute>();
        }

        public IQueryable<Attribute> GetAll(string? text = null, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToLower();
                query = query.Where(x =>
                x.Name.ToLower().Contains(text) ||
                    x.Name.ToLower().Contains(text) ||
                    x.Id.ToString().Contains(text));
            }

            return query;
        }

        public IQueryable<Attribute> GetWhere(Expression<Func<Attribute, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public async Task<Attribute?> GetSingleAsync(Expression<Func<Attribute, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Attribute?> GetByIdAsync(Guid id, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(Attribute model)
        {
            await _dbSet.AddAsync(model);
            return await SaveAsync();
        }

        public async Task<bool> AddRangeAsync(List<Attribute> models)
        {
            await _dbSet.AddRangeAsync(models);
            return await SaveAsync();
        }

        public bool Remove(Attribute model)
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

        public bool RemoveRange(List<Attribute> models)
        {
            _dbSet.RemoveRange(models);
            return SaveAsync().Result;
        }

        public bool Update(Attribute model)
        {
            _dbSet.Update(model);
            return SaveAsync().Result;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }          
    }
}
