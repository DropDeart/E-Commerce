using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAll(string? text = null, bool tracking = true);
        IQueryable<Product> GetWhere(Expression<Func<Product, bool>> predicate, bool tracking = true);
        Task<Product?> GetSingleAsync(Expression<Func<Product, bool>> predicate, bool tracking = true);
        Task<Product?> GetByIdAsync(Guid id, bool tracking = true);
        Task<bool> AddAsync(Product model);
        Task<bool> AddRangeAsync(List<Product> models);
        bool Remove(Product model);
        Task<bool> RemoveAsync(Guid id);
        bool RemoveRange(List<Product> models);
        bool Update(Product model);
        Task<bool> SaveAsync();

        // Özel methodlar
        IQueryable<Product> GetProductsWithCategory(bool tracking = true);
        Task<Product?> GetProductDetailsAsync(Guid id, bool tracking = true);
        IQueryable<Product> GetLowStockProducts(int threshold, bool tracking = true);
    }
}
