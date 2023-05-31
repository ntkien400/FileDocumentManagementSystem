using FileDocument.DataAccess.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FileDocument.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            this.dbSet = _dbContext.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null
            , Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null
            , string properties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }
            if(orderBy != null)
            {
                return orderBy(query).ToList();
            }
            if(properties != null)
            {
                foreach(var property in properties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null
            , string properties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (properties != null)
            {
                foreach (var property in properties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.FirstAsync();
        }

    }
}
