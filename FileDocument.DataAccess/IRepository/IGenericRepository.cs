using System.Linq.Expressions;

namespace FileDocument.DataAccess.IRepository
{
    public interface IGenericRepository<T> where T:class
    {
        Task<T> GetAsync (Expression<Func<T, bool>> filter =null
            , string properties = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null
            , Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null
            , string properties = null);
        void Add(T entity);
        Task AddAsync(T entity);
        void Update(T entity);  
        void Delete(T entity);  
    }
}
