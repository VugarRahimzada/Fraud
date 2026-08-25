using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fraud.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<PagedResult<T>> GetPagedAsync(
            PaginationParams paginationParams,
            Expression<Func<T, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, paginationParams);

            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<T> ApplySorting(IQueryable<T> query, PaginationParams paginationParams)
        {
            if (string.IsNullOrWhiteSpace(paginationParams.SortBy))
            {
                return paginationParams.SortDescending
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id);
            }

            var property = typeof(T).GetProperty(
                paginationParams.SortBy,
                System.Reflection.BindingFlags.IgnoreCase
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance);

            if (property == null)
            {
                return paginationParams.SortDescending
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id);
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = paginationParams.SortDescending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDelete = true;
            entity.UpdateDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}