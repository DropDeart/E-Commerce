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
using static System.Net.Mime.MediaTypeNames;

namespace LJussAPI.Persistance.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Product> _dbSet;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Product>();
        }

        public IQueryable<Product> GetAll(string? text = null,bool tracking = true)
        {
            var query = _dbSet.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            query = query.Include(P => P.Category);

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

        public IQueryable<Product> GetWhere(Expression<Func<Product, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public async Task<Product?> GetSingleAsync(Expression<Func<Product, bool>> predicate, bool tracking = true)
        {
            var query = _dbSet.Where(predicate);
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id, bool tracking = true)
        {
            var query = _dbSet.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            query = query
                .Include(p => p.Category)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.Attribute)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User);

            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(Product model)
        {
            await _dbSet.AddAsync(model);
            return await SaveAsync();
        }

        public async Task<bool> AddRangeAsync(List<Product> models)
        {
            await _dbSet.AddRangeAsync(models);
            return await SaveAsync();
        }

        public bool Remove(Product model)
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

        public bool RemoveRange(List<Product> models)
        {
            _dbSet.RemoveRange(models);
            return SaveAsync().Result;
        }

        public bool Update(Product model)
        {
            _dbSet.Update(model);
            return SaveAsync().Result;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public IQueryable<Product> GetProductsWithCategory(bool tracking = true)
        {
            var query = _dbSet.Include(p => p.Category);
            if (!tracking)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, Category>)query.AsNoTracking();
            return query;
        }

        public async Task<Product?> GetProductDetailsAsync(Guid id, bool tracking = true)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductAttributes)
                .Include(p => p.ProductImages)
                .Include(p => p.Comments)
                .Include(p => p.Favorites)
                .AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public IQueryable<Product> GetLowStockProducts(int threshold, bool tracking = true)
        {
            var query = _dbSet.Where(p => p.Stock <= threshold);
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }
    }
}
